using DataAllyEngine.Models;
using FacebookLoader.Common;

namespace DataAllyEngine.LoaderTask;

public interface ILoaderRunner
{
	void StartAdCreativesLoad(FacebookParameters facebookParameters, Channel channel, string scopeType);

	void StartAdCreativesLoad(FacebookParameters facebookParameters, FbRunLog runlog);
	
	void ResumeAdCreativesLoad(FacebookParameters facebookParameters, FbRunLog runlog, string url);

	void StartAdImagesLoad(FacebookParameters facebookParameters, Channel channel, string scopeType);

	void StartAdImagesLoad(FacebookParameters facebookParameters, FbRunLog runlog);

	void ResumeAdImagesLoad(FacebookParameters facebookParameters, FbRunLog runlog, string url);

	void StartAdInsightsLoad(FacebookParameters facebookParameters, Channel channel, string scopeType, DateTime startDate, DateTime endDate);

	void StartAdInsightsLoad(FacebookParameters facebookParameters, FbRunLog runlog, DateTime startDate, DateTime endDate);

	void ResumeAdInsightsLoad(FacebookParameters facebookParameters, FbRunLog runlog, string url);
}