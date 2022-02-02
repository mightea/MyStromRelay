
using System.Threading.Tasks;

namespace MyStromRelay.Models
{
    public interface IReporterModel
    {
        internal Task ReportButtonPress(string buttonId, string action);

        internal Task ReportBatteryStatus(string buttonId, int level);
    }
}