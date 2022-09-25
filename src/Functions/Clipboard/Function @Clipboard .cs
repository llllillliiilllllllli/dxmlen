using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCopy;

namespace DxMLEngine.Functions
{
    internal class Clipboard
    {
        public static async Task<string?> GetTextAsync()
        {
            return await ClipboardService.GetTextAsync();
        }        
        
        public static async Task SetTextAsync(string path, string text)
        {
            await ClipboardService.SetTextAsync(text);
        }        
        
        public static string? GetText()
        {
            return ClipboardService.GetText();
        }        
        
        public static void SetText(string path, string text)
        {
            ClipboardService.SetText(text);
        }
    }
}
