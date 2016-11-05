using System;
using Framework;

namespace Plugin1
{
    public class Plugin1 : IPlugin
    {
        public string Name
        {
            get
            {
                return "Plugin1";
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
