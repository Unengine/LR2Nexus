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

	public MD5Hash CalculateScorehash(MD5Hash passwordMD5)
	{
		return MD5Util.CalculateMD5(ToRawArray(passwordMD5));
	}

	public void UpdateScorehash(MD5Hash passwordMD5)
	{
		Scorehash = CalculateScorehash(passwordMD5).Body;
	}

	public object[] ToRawArray(MD5Hash passwordMD5)
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
			passwordMD5.Body,
            Hash ?? string.Empty,
            "1"
        ];
	}

	public static bool IsSameScoreHash(DBScoreRow score, MD5Hash passwordHash, MD5Hash targetHash)
	{
		MD5Hash newHash = score.CalculateScorehash(passwordHash);
		if (newHash == targetHash) return true;

		if (score.Perfect == score.Totalnotes && score.Rank == 8)
		{
			score.Rank = 9;
			newHash = score.CalculateScorehash(passwordHash);
			if (newHash == targetHash)
			{
				score.Rank = 8;
				return true;
			}
		}
		if (score.Perfect == score.Totalnotes && score.Rank == 9)
		{
			score.Rank = 8;
			newHash = score.CalculateScorehash(passwordHash);
			if (newHash == targetHash)
			{
				return true;
			}
		}

		if (score.Rank == 1)
		{
			score.Rank = 0;
			newHash = score.CalculateScorehash(passwordHash);
			if (newHash == targetHash)
			{
				score.Rank = 1;
				return true;
			}
		}
		if (score.Rank == 0)
		{
			score.Rank = 1;
			newHash = score.CalculateScorehash(passwordHash);
			if (newHash == targetHash)
			{
				return true;
			}
		}

		return false;
	}
}