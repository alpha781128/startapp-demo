using System;
using System.Collections.Generic;
using System.Text;

namespace Startapp.Shared
{
    public static class Functions
    {
        public static int RemainingTime(DateTime expireDate)
        {
            // 120 seconds (2) minutes before refresh token expired
            return (int)(expireDate - DateTime.UtcNow).TotalSeconds - 120;  
        }
    }
}
