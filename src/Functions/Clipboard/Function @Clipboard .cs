using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCopy;

namespace DxMLEngine.Functions
{
    internal class DxClipboard
    {
        public static async Task<string?> GetTextAsync()
        {
            return await ClipboardService.GetTextAsync();
        }        
        
        public static async Task SetTextAsync(string text)
        {
            await ClipboardService.SetTextAsync(text);
        }        
        
        public static string GetText()
        {
            var text = ClipboardService.GetText();
            if (text != null) return text;
            else { return string.Empty; }
        }        
        
        public static void SetText(string text)
        {
            ClipboardService.SetText(text);
        }
    }
}
