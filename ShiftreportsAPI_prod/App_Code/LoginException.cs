using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpensTrackerAPI.App_Code
{
    public class LoginException : Exception
    {
        private string p;

        public LoginException(string p)
        {
            // TODO: Complete member initialization
            this.p = p;
        }
    }
}
