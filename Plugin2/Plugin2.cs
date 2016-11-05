using System;
using Framework;

namespace Plugin2
{
    public class Plugin2 : IPlugin
    {
        public string Name
        {
            get
            {
                return "Plugin2";
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
