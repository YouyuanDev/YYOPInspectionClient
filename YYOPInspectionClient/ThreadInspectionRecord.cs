using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYOPInspectionClient
{
    public class ThreadInspectionRecord
    {
        private string id;
        private string thread_inspection_record_code;
        private string coupling_no;
        private string contract_no;
        private string production_line;
        private string machine_no;
        private string process_no;
        private string operator_no;
        private string production_crew;
        private string production_shift;
        private string video_no;
        private string inspection_time;
        private string inspection_result;
        private string coupling_heat_no;
        private string coupling_lot_no;
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

        public string Thread_inspection_record_code
        {
            get
            {
                return thread_inspection_record_code;
            }

            set
            {
                thread_inspection_record_code = value;
            }
        }

         
        public string Contract_no
        {
            get
            {
                return contract_no;
            }

            set
            {
                contract_no = value;
            }
        }

        public string Production_line
        {
            get
            {
                return production_line;
            }

            set
            {
                production_line = value;
            }
        }

        public string Machine_no
        {
            get
            {
                return machine_no;
            }

            set
            {
                machine_no = value;
            }
        }

        public string Process_no
        {
            get
            {
                return process_no;
            }

            set
            {
                process_no = value;
            }
        }

        public string Operator_no
        {
            get
            {
                return operator_no;
            }

            set
            {
                operator_no = value;
            }
        }

        public string Production_crew
        {
            get
            {
                return production_crew;
            }

            set
            {
                production_crew = value;
            }
        }

        public string Production_shift
        {
            get
            {
                return production_shift;
            }

            set
            {
                production_shift = value;
            }
        }

        public string Video_no
        {
            get
            {
                return video_no;
            }

            set
            {
                video_no = value;
            }
        }

        public string Inspection_time
        {
            get
            {
                return inspection_time;
            }

            set
            {
                inspection_time = value;
            }
        }

        public string Inspection_result
        {
            get
            {
                return inspection_result;
            }

            set
            {
                inspection_result = value;
            }
        }

        public string Coupling_lot_no
        {
            get
            {
                return coupling_lot_no;
            }

            set
            {
                coupling_lot_no = value;
            }
        }

        public string Coupling_heat_no
        {
            get
            {
                return coupling_heat_no;
            }

            set
            {
                coupling_heat_no = value;
            }
        }

        public string Coupling_no
        {
            get
            {
                return coupling_no;
            }

            set
            {
                coupling_no = value;
            }
        }
    }
}
