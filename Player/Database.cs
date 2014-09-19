using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;

namespace L2Apf
{
	sealed class Database
	{
		public Database(string file)
		{
			string cs = "Data Source=|DataDirectory|\\" + file;
			if (!System.IO.File.Exists(file))
				Create(cs);

			Connection = new SqlCeConnection(cs);
			Connection.Open();
		}

		private void Create(string cs)
		{
			throw new NotImplementedException();

			// SqlCeEngine en = new SqlCeEngine(connectionString);
			// en.CreateDatabase();

			// create tables's
			// generate & insert default values
		}

		public Model.Memory.Nature GetNatue()
		{
			SqlCeCommand command = Connection.CreateCommand();
			command.CommandText = "SELECT TOP(1) * FROM [nature]";
			SqlCeDataReader reader = command.ExecuteReader();
			reader.Read();

			return new Model.Memory.Nature()
			{
				Intellect = reader.GetDouble(reader.GetOrdinal("intellect")),
				Courage = reader.GetDouble(reader.GetOrdinal("courage")),
				Godliness = reader.GetDouble(reader.GetOrdinal("godliness")),
				Sociality = reader.GetDouble(reader.GetOrdinal("sociality")),
				Peaceness = reader.GetDouble(reader.GetOrdinal("peaceness")),
				Curiosity = reader.GetDouble(reader.GetOrdinal("curiosity")),
				Hunting = reader.GetDouble(reader.GetOrdinal("hunting")),
				Volatility = reader.GetDouble(reader.GetOrdinal("volatility")),
				Forgetness = reader.GetDouble(reader.GetOrdinal("forgetness")),
			};
		}

		public void SetNature(Model.Memory.Nature nature)
		{
			throw new NotImplementedException();
		}

		public Model.Memory.Config GetConfig()
		{
			SqlCeCommand command = Connection.CreateCommand();
			command.CommandText = "SELECT TOP(1) * FROM [config]";
			SqlCeDataReader reader = command.ExecuteReader();
			reader.Read();

			byte[] token = new byte[21];
			reader.GetBytes(reader.GetOrdinal("token"), 0, token, 0, 21);

			return new Model.Memory.Config()
			{
				Address = reader.GetString(reader.GetOrdinal("address")),
				Port = reader.GetInt32(reader.GetOrdinal("port")),
				Protocol = reader.GetInt32(reader.GetOrdinal("protocol")),
				Token = token,
				Login = reader.GetString(reader.GetOrdinal("login")),
				Password = reader.GetString(reader.GetOrdinal("password")),
				ServerId = reader.GetInt32(reader.GetOrdinal("server")),
				Name = reader.GetString(reader.GetOrdinal("name"))
			};
		}


		private SqlCeConnection Connection;
	}
}
