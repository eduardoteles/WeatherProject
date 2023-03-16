namespace WeatherProject.Models
{
    public class Forecast
    {
        private int id;
        private DateTime date;
        private string description;
        private float tempMax;
        private float tempMin;

        public int Id { get => id; set => id = value; }
        public DateTime Date { get => date; set => date = value; }
        public string Description { get => description; set => description = value; }
        public float TempMax { get => tempMax; set => tempMax = value; }
        public float TempMin { get => tempMin; set => tempMin = value; }
    }
}
