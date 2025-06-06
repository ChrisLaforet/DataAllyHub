using DataAllyEngine.Models;

namespace DataAllyEngine.Proxy;

public interface ILoaderProxy
{
	Channel? GetChannelById(int channelId);
	
	ChannelType? GetChannelTypeByName(string channelName);
	
	Channel? GetChannelByChannelAccountId(string channelAccountId);
	
	FbDailySchedule? GetFbDailyScheduleByChannelId(int channelId);
	void WriteFbDailySchedule(FbDailySchedule dailySchedule);
	
	FbRunLog? GetFbRunLogById(int runlogId);
	List<FbRunLog> GetFbRunLogsByChannelIdAfterDate(int channelId, DateTime date);
	void WriteFbRunLog(FbRunLog runLog);
	
	void WriteFbSaveContent(FbSaveContent saveContent);
	
	void WriteFbRunProblem(FbRunProblem runProblem);
	
	int GetNextSequenceByRunlogId(int runlogId);
	void WriteFbRunStaging(FbRunStaging runStaging);
	
	Token? GetTokenByCompanyIdAndChannelTypeId(int companyId, int channelTypeId);

	List<FbCreativeLoad> GetPendingCreativeImages(int startId = 0, int batchSize = 1000);
	List<FbCreativeLoad> GetPendingCreativeVideos(int startId = 0, int batchSize = 1000);
	
	void WriteFbCreativeLoad(FbCreativeLoad creativeLoad);
}