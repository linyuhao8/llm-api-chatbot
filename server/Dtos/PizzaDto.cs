using System.ComponentModel.DataAnnotations;
namespace ContosoPizza.Dtos
{
    public class CreatePizzaDto
    {
        [Required(ErrorMessage = "名稱必填")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "名稱長度需介於 3 到 50 字元")]
        public string Name { get; set; } = string.Empty;
        [Range(1, 5000, ErrorMessage = "價格必須介於 1 到 5000")]
        public decimal Price { get; set; }

        // 可選：保留空的建構子
        public CreatePizzaDto() { }
    }
    
    public class UpdatePizzaDto
    {
        [Required(ErrorMessage = "名稱必填")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "名稱長度需介於 3 到 50 字元")]
        public string Name { get; set; } = string.Empty;
        [Range(1, 5000, ErrorMessage = "價格必須介於 1 到 5000")]
        public decimal Price { get; set; }

        // 可選：保留空的建構子
        public UpdatePizzaDto() { }
    }
}
