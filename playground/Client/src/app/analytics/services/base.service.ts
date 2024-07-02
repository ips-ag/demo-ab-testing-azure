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
  async calculateSHA256(inputString: string): Promise<string> {
    // Step 1: Convert the string to an ArrayBuffer
    const encoder = new TextEncoder();
    const data = encoder.encode(inputString);

    // Step 2: Calculate the SHA-256 hash
    const hashBuffer = await crypto.subtle.digest('SHA-256', data);

    // Step 3: Convert the ArrayBuffer to a hexadecimal string
    const hashArray = Array.from(new Uint8Array(hashBuffer));
    const hashHex = hashArray
      .map((b) => b.toString(16).padStart(2, '0'))
      .join('');

    return hashHex;
  }
}
