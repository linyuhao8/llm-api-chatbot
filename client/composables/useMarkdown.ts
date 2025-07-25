// composables/useMarkdown.ts
import MarkdownIt from "markdown-it";
import hljs from "highlight.js";
import "highlight.js/styles/github-dark.css"; // 可換成你喜歡的主題

const md: MarkdownIt = new MarkdownIt({
  html: true,
  linkify: true,
  typographer: true,
  highlight: (str: string, lang: string) => {
    if (lang && hljs.getLanguage(lang)) {
      return `<pre class="hljs" style="padding:20px;border-radius:20px;"><code>${
        hljs.highlight(str, { language: lang }).value
      }</code></pre>`;
    }
    return `<pre class="hljs"><code>${md.utils.escapeHtml(str)}</code></pre>`;
  },
});

export function useMarkdown() {
  const render = (text: string) => md.render(text);
  return { render };
}
