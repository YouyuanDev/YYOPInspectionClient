using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYOPInspectionClient
{
    public class ThreadInspectionRecord
    {
        private int id;
        private String thread_inspection_record_code;
        private String couping_no;
        private String contract_no;
        private String production_line;
        private String machine_no;
        private String process_no;
        private String operator_no;
        private String production_crew;
        private String production_shift;
        private String video_no;
        private DateTime inspection_time;
        private String inspection_result;

        public int Id
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

        public string Couping_no
        {
            get
            {
                return couping_no;
            }

            set
            {
                couping_no = value;
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

        public DateTime Inspection_time
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
    }
}
