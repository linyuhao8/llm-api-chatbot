// EF ORM
// _context.Pizzas.ToList();                // SELECT * FROM Pizzas
// _context.Pizzas.Add(new Pizza { ... }); // INSERT INTO Pizzas ...
// _context.Pizzas.Find(1);                // SELECT * FROM Pizzas WHERE Id = 1
// _context.Pizzas.Remove(pizza);          // DELETE FROM Pizzas WHERE ...


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoPizza.Models;
using ContosoPizza.Data;
using ContosoPizza.Dtos;

[ApiController]
[Route("api/[controller]")]

//PizzaController 繼承自 ControllerBase，代表這是一個 Web API 控制器，不會有 View（前端頁面）。
public class PizzaController : ControllerBase
{
    // 透過建構子注入 AppDbContext，
    // 取得資料庫操作的上下文 _context，用來讀寫資料。
    private readonly AppDbContext _context;

    public PizzaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var pizzas = await _context.Pizzas.ToListAsync();
        return Ok(pizzas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var pizza = await _context.Pizzas.FindAsync(id);
        if (pizza == null) return NotFound(); //404
        return Ok(pizza);
    }

    [HttpPost]
    //新增一筆 Pizza（由前端送來 JSON，自動轉成 Pizza 物件）。
    //加到 EF Core 追蹤的 Pizzas 集合。
    public async Task<IActionResult> Create([FromBody] CreatePizzaDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var pizza = new Pizza
        {
            Name = dto.Name,
            Price = dto.Price
        };
        _context.Pizzas.Add(pizza);
        await _context.SaveChangesAsync();
        
        // 回傳 HTTP 201 Created，並且在回應標頭中
        // 告訴前端「新增的資源位置」(Location header) 指向剛新增的 pizza。
        return CreatedAtAction(nameof(Get), new { id = pizza.Id }, pizza);
    }

[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdatePizzaDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var pizza = await _context.Pizzas.FindAsync(id);
    if (pizza == null)
        return NotFound();

    // 不管 body 裡 pizza.Id 是多少，強制用 URL id 找到的資料更新
    pizza.Name = dto.Name;
    pizza.Price = dto.Price;
    // 如果你想要把 pizza.Id 寫回資料庫，最好就不在此更新 id 欄位

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!await _context.Pizzas.AnyAsync(e => e.Id == id))
            return NotFound();
        else
            throw;
    }

    return NoContent();
}



    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        //先找資料，不存在回 404。
        var pizza = await _context.Pizzas.FindAsync(id);
        if (pizza == null) return NotFound();

        //刪除
        _context.Pizzas.Remove(pizza);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
