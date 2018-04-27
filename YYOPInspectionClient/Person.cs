using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYOPInspectionClient
{
    public class Person
    {
        public static string id;
        public static String employee_no;
        public static String pname;
        private static String ppassword;
        private static String pidcard_no;
        private static String pmobile;
        private static string page;
        private static String psex;
        private static String pstatus;
        private static String pdepartment;
        private static string pregister_time;
        private static String role_no_list;

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

        public string Employee_no
        {
            get
            {
                return employee_no;
            }

            set
            {
                employee_no = value;
            }
        }

        public string Pname
        {
            get
            {
                return pname;
            }
            set
            {
                pname = value;
            }
        }

        public string Ppassword
        {
            get
            {
                return ppassword;
            }

            set
            {
                ppassword = value;
            }
        }

        public string Pidcard_no
        {
            get
            {
                return pidcard_no;
            }

            set
            {
                pidcard_no = value;
            }
        }

        public string Pmobile
        {
            get
            {
                return pmobile;
            }

            set
            {
                pmobile = value;
            }
        }

        public string Page
        {
            get
            {
                return page;
            }

            set
            {
                page = value;
            }
        }

        public string Psex
        {
            get
            {
                return psex;
            }

            set
            {
                psex = value;
            }
        }

        public string Pstatus
        {
            get
            {
                return pstatus;
            }

            set
            {
                pstatus = value;
            }
        }

        public string Pdepartment
        {
            get
            {
                return pdepartment;
            }

            set
            {
                pdepartment = value;
            }
        }

        public string Pregister_time
        {
            get
            {
                return pregister_time;
            }

            set
            {
                pregister_time = value;
            }
        }

        public string Role_no_list
        {
            get
            {
                return role_no_list;
            }

            set
            {
                role_no_list = value;
            }
        }
    }
}
