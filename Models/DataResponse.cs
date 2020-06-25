using System.Data;

namespace InnovativeService.Models
{
    public class DataResponse
    {
        public DataTable Records { get; set; }

        public string Status { get; set; }

        public string Message { get; set; }
    }
}