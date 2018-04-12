using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YYOPInspectionClient
{
    public class ThreadingProcess
    {
        private int id;
        private String couping_no;
        private String process_no;
        private String operator_no;
        private string inspection_time;
        private String visual_inspection;
        private float thread_tooth_pitch_diameter_max;
        private float thread_tooth_pitch_diameter_avg;
        private float thread_tooth_pitch_diameter_min;
        private float thread_sealing_surface_diameter_max;
        private float thread_sealing_surface_diameter_avg;
        private float thread_sealing_surface_diameter_min;
        private float thread_sealing_surface_ovality;
        private float thread_width;
        private float thread_pitch;
        private float thread_taper;
        private float thread_height;
        private float thread_length_min;
        private float thread_bearing_surface_width;
        private float couping_inner_end_depth;
        private float thread_hole_inner_diameter;
        private float couping_od;
        private float couping_length;
        private float thread_tooth_angle;
        private float thread_throug_hole_size;
        private String video_no;
        private String tool_measuring_record_no;

        private String thread_pitch_gauge_no;
        private String thread_pitch_calibration_framwork;
        private String sealing_surface_gauge_no;
        private String sealing_surface_calibration_ring_no;
        private String depth_caliper_no;
        private String threading_distance_gauge_no;
        private String thread_distance_calibration_sample_no;
        private String taper_gauge_no;
        private String tooth_height_gauge_no;
        private String tooth_height_calibration_sample_no;
        private String tooth_width_stop_gauge_no;
        private String thread_min_length_sample_no;
        private String coupling_length_sample_no;
        private String caliper_no;
        private String caliper_tolerance;
        private String collar_gauge_no;
        private String thread_acceptance_criteria_no;
        private string contract_no;
        private string heat_no;
        private string test_batch_no;
        private string steel_grade;
        private string texture;
        private string production_area;
        private string machine_no;
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

        public string Visual_inspection
        {
            get
            {
                return visual_inspection;
            }

            set
            {
                visual_inspection = value;
            }
        }

        public float Thread_tooth_pitch_diameter_max
        {
            get
            {
                return thread_tooth_pitch_diameter_max;
            }

            set
            {
                thread_tooth_pitch_diameter_max = value;
            }
        }

        public float Thread_tooth_pitch_diameter_avg
        {
            get
            {
                return thread_tooth_pitch_diameter_avg;
            }

            set
            {
                thread_tooth_pitch_diameter_avg = value;
            }
        }

        public float Thread_tooth_pitch_diameter_min
        {
            get
            {
                return thread_tooth_pitch_diameter_min;
            }

            set
            {
                thread_tooth_pitch_diameter_min = value;
            }
        }

        public float Thread_sealing_surface_diameter_max
        {
            get
            {
                return thread_sealing_surface_diameter_max;
            }

            set
            {
                thread_sealing_surface_diameter_max = value;
            }
        }

        public float Thread_sealing_surface_diameter_avg
        {
            get
            {
                return thread_sealing_surface_diameter_avg;
            }

            set
            {
                thread_sealing_surface_diameter_avg = value;
            }
        }

        public float Thread_sealing_surface_diameter_min
        {
            get
            {
                return thread_sealing_surface_diameter_min;
            }

            set
            {
                thread_sealing_surface_diameter_min = value;
            }
        }

        public float Thread_sealing_surface_ovality
        {
            get
            {
                return thread_sealing_surface_ovality;
            }

            set
            {
                thread_sealing_surface_ovality = value;
            }
        }

        public float Thread_width
        {
            get
            {
                return thread_width;
            }

            set
            {
                thread_width = value;
            }
        }

        public float Thread_pitch
        {
            get
            {
                return thread_pitch;
            }

            set
            {
                thread_pitch = value;
            }
        }

        public float Thread_taper
        {
            get
            {
                return thread_taper;
            }

            set
            {
                thread_taper = value;
            }
        }

        public float Thread_height
        {
            get
            {
                return thread_height;
            }

            set
            {
                thread_height = value;
            }
        }

        public float Thread_length_min
        {
            get
            {
                return thread_length_min;
            }

            set
            {
                thread_length_min = value;
            }
        }

        public float Thread_bearing_surface_width
        {
            get
            {
                return thread_bearing_surface_width;
            }

            set
            {
                thread_bearing_surface_width = value;
            }
        }

        public float Couping_inner_end_depth
        {
            get
            {
                return couping_inner_end_depth;
            }

            set
            {
                couping_inner_end_depth = value;
            }
        }

        public float Thread_hole_inner_diameter
        {
            get
            {
                return thread_hole_inner_diameter;
            }

            set
            {
                thread_hole_inner_diameter = value;
            }
        }

        public float Couping_od
        {
            get
            {
                return couping_od;
            }

            set
            {
                couping_od = value;
            }
        }

        public float Couping_length
        {
            get
            {
                return couping_length;
            }

            set
            {
                couping_length = value;
            }
        }

        public float Thread_tooth_angle
        {
            get
            {
                return thread_tooth_angle;
            }

            set
            {
                thread_tooth_angle = value;
            }
        }

        

        public float Thread_throug_hole_size
        {
            get
            {
                return thread_throug_hole_size;
            }

            set
            {
                thread_throug_hole_size = value;
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

        public string Tool_measuring_record_no
        {
            get
            {
                return tool_measuring_record_no;
            }

            set
            {
                tool_measuring_record_no = value;
            }
        }

        public string Inspection_result
        {
            get
            {
                return Inspection_result1;
            }

            set
            {
                Inspection_result1 = value;
            }
        }

        public string Thread_pitch_gauge_no
        {
            get
            {
                return thread_pitch_gauge_no;
            }

            set
            {
                thread_pitch_gauge_no = value;
            }
        }

        public string Thread_pitch_calibration_framwork
        {
            get
            {
                return thread_pitch_calibration_framwork;
            }

            set
            {
                thread_pitch_calibration_framwork = value;
            }
        }

        public string Sealing_surface_gauge_no
        {
            get
            {
                return sealing_surface_gauge_no;
            }

            set
            {
                sealing_surface_gauge_no = value;
            }
        }

        public string Sealing_surface_calibration_ring_no
        {
            get
            {
                return sealing_surface_calibration_ring_no;
            }

            set
            {
                sealing_surface_calibration_ring_no = value;
            }
        }

        public string Depth_caliper_no
        {
            get
            {
                return depth_caliper_no;
            }

            set
            {
                depth_caliper_no = value;
            }
        }

        public string Threading_distance_gauge_no
        {
            get
            {
                return threading_distance_gauge_no;
            }

            set
            {
                threading_distance_gauge_no = value;
            }
        }

        public string Thread_distance_calibration_sample_no
        {
            get
            {
                return thread_distance_calibration_sample_no;
            }

            set
            {
                thread_distance_calibration_sample_no = value;
            }
        }

        public string Taper_gauge_no
        {
            get
            {
                return taper_gauge_no;
            }

            set
            {
                taper_gauge_no = value;
            }
        }

        public string Tooth_height_gauge_no
        {
            get
            {
                return tooth_height_gauge_no;
            }

            set
            {
                tooth_height_gauge_no = value;
            }
        }

        public string Tooth_height_calibration_sample_no
        {
            get
            {
                return tooth_height_calibration_sample_no;
            }

            set
            {
                tooth_height_calibration_sample_no = value;
            }
        }

        public string Tooth_width_stop_gauge_no
        {
            get
            {
                return tooth_width_stop_gauge_no;
            }

            set
            {
                tooth_width_stop_gauge_no = value;
            }
        }

        public string Thread_min_length_sample_no
        {
            get
            {
                return thread_min_length_sample_no;
            }

            set
            {
                thread_min_length_sample_no = value;
            }
        }

        public string Coupling_length_sample_no
        {
            get
            {
                return coupling_length_sample_no;
            }

            set
            {
                coupling_length_sample_no = value;
            }
        }

        public string Caliper_no
        {
            get
            {
                return caliper_no;
            }

            set
            {
                caliper_no = value;
            }
        }

        public string Caliper_tolerance
        {
            get
            {
                return caliper_tolerance;
            }

            set
            {
                caliper_tolerance = value;
            }
        }

        public string Collar_gauge_no
        {
            get
            {
                return collar_gauge_no;
            }

            set
            {
                collar_gauge_no = value;
            }
        }

        public string Thread_acceptance_criteria_no
        {
            get
            {
                return thread_acceptance_criteria_no;
            }

            set
            {
                thread_acceptance_criteria_no = value;
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

        public string Heat_no
        {
            get
            {
                return heat_no;
            }

            set
            {
                heat_no = value;
            }
        }

        public string Test_batch_no
        {
            get
            {
                return test_batch_no;
            }

            set
            {
                test_batch_no = value;
            }
        }

        public string Steel_grade
        {
            get
            {
                return steel_grade;
            }

            set
            {
                steel_grade = value;
            }
        }

        public string Texture
        {
            get
            {
                return texture;
            }

            set
            {
                texture = value;
            }
        }

        public string Production_area
        {
            get
            {
                return production_area;
            }

            set
            {
                production_area = value;
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

        public string Inspection_result1
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
