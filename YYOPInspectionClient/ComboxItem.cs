using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYOPInspectionClient
{
    public  class ComboxItem
    {
        private string id;
        private string text;

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }
    }
}
