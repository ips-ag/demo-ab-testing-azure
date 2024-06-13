export class BaseService {
  addScriptToHead(src?: string, content?: string): void {
    const script = document.createElement('script');
    if (content) {
      script.innerHTML = content;
    } else if (src) {
      script.src = src;
    }
    document.head.appendChild(script);
  }
}
