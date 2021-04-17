using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Startapp.Client.Helpers
{
    public static class IJSRuntimeExtensionMethods
    {
        public static async ValueTask WriteToConsole(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("console.log", message);
        }

        public static async ValueTask InvokeMethod(this IJSRuntime js, string MethodName)
        {
            await js.InvokeVoidAsync(MethodName);
        }

    }

    

}
