using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobinhoodDataUtility
{
    class Wrappers
    {
        public class ApiTokenAuth
        {
            public string token { get; set; }
        }

        public class Execution
        {
            public DateTime timestamp { get; set; }
            public string price { get; set; }
            public string settlement_date { get; set; }
            public string id { get; set; }
            public string quantity { get; set; }
        }

        public class Result
        {
            public DateTime updated_at { get; set; }
            public string ref_id { get; set; }
            public string time_in_force { get; set; }
            public string fees { get; set; }
            public object cancel { get; set; }
            public string id { get; set; }
            public string cumulative_quantity { get; set; }
            public string stop_price { get; set; }
            public object reject_reason { get; set; }
            public string instrument { get; set; }
            public string state { get; set; }
            public string trigger { get; set; }
            public bool override_dtbp_checks { get; set; }
            public string type { get; set; }
            public DateTime last_transaction_at { get; set; }
            public string price { get; set; }
            public IList<Execution> executions { get; set; }
            public bool extended_hours { get; set; }
            public string account { get; set; }
            public string url { get; set; }
            public DateTime created_at { get; set; }
            public string side { get; set; }
            public bool override_day_trade_checks { get; set; }
            public string position { get; set; }
            public string average_price { get; set; }
            public string quantity { get; set; }
        }

        public class Example
        {
            public object previous { get; set; }
            public IList<Result> results { get; set; }
            public string next { get; set; }
        }
    }
}
