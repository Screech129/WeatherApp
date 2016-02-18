using SQLite;


namespace WeatherApp
{
	public class BaseColumns
	{
		public BaseColumns ()
		{
			
		}

		[PrimaryKey,AutoIncrement,Column ("_id")]
		public int Id { get; set; }

		public int Count{ get; set; }
	}
}

