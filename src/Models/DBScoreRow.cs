using LR2Nexus.Utils;
using Microsoft.Data.Sqlite;

namespace LR2Nexus.Models;

public class DBScoreRow(SqliteDataReader reader)
{
	public string? Hash { get; set; } = DatabaseUtils.GetValue<string>(reader, "hash");
	public int Clear { get; set; } = DatabaseUtils.GetValue<int>(reader, "clear");
	public int Perfect { get; set; } = DatabaseUtils.GetValue<int>(reader,"perfect");
	public int Great { get; set; } = DatabaseUtils.GetValue<int>(reader,"great");
	public int Good { get; set; } = DatabaseUtils.GetValue<int>(reader, "good");
	public int Bad { get; set; } = DatabaseUtils.GetValue<int>(reader, "bad");
	public int Poor { get; set; } = DatabaseUtils.GetValue<int>(reader, "poor");
	public int Totalnotes { get; set; } = DatabaseUtils.GetValue<int>(reader, "totalnotes");
	public int Maxcombo { get; set; } = DatabaseUtils.GetValue<int>(reader, "maxcombo");
	public int Minbp { get; set; } = DatabaseUtils.GetValue<int>(reader, "minbp");
	public int Playcount { get; set; } = DatabaseUtils.GetValue<int>(reader, "playcount");
	public int Clearcount { get; set; } = DatabaseUtils.GetValue<int>(reader, "clearcount");
	public int Failcount { get; set; } = DatabaseUtils.GetValue<int>(reader, "failcount");
	public int Rank { get; set; } = DatabaseUtils.GetValue<int>(reader, "rank");
	public int Rate { get; set; } = DatabaseUtils.GetValue<int>(reader, "rate");
	public int ClearDb { get; set; } = DatabaseUtils.GetValue<int>(reader, "clear_db");
	public int OpHistory { get; set; } = DatabaseUtils.GetValue<int>(reader, "op_history");
	public string? Scorehash { get; set; } = DatabaseUtils.GetValue<string>(reader, "scorehash");

	public string CalculateScorehash(string password)
	{
		return MD5Util.CalculateMD5(ToRawArray(password));
	}

	public void UpdateScorehash(string password)
	{
		Scorehash = CalculateScorehash(password);
	}

	public object[] ToRawArray(string password)
	{
		return
		[
			Playcount,
            Perfect,
            Poor,
            Good,
            Great,
            Bad,
            Totalnotes,
            Clear,
            Maxcombo,
            Clearcount,
            Failcount,
            Rank,
            Minbp,
            Rate,
            ClearDb,
            OpHistory,
            password,
            Hash ?? string.Empty,
            "1"
        ];
	}
}