namespace FribergCarRentals.Models
{
    public class Log
    {
        public int LogId { get; set; }
        public string LogText { get; set; }
        public DateTime LogDate { get; set; }

        public Log() { }
        public Log(string logText)
        {
            LogText = logText;
            LogDate = DateTime.Now;
        }

    }
}
