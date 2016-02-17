using SQLite;


namespace WeatherApp
{
	public class BaseColumns
	{
		public BaseColumns ()
		{
			Id = 0;
			Count = 0;
		}

		[PrimaryKey,AutoIncrement,Column ("_id")]
		int Id { get; set; }

		int Count{ get; set; }
	}
}

