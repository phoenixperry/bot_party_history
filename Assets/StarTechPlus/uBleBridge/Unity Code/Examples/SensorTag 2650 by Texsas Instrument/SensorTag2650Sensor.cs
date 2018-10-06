using System.Collections;

namespace startechplus.ble.examples
{
	public class SensorTag2650Sensor {

		public string DataUuid;
		public string ConfigurationUuid;
		public string PeriodUuid;

		public SensorTag2650Sensor(string baseUuid, string dataUuid, string configurationUuid, string periodUuid)
		{
			string prefix = baseUuid.Substring(0, 4);
			string suffix = baseUuid.Substring(8);

			DataUuid = prefix + dataUuid + suffix;
			ConfigurationUuid = prefix + configurationUuid + suffix;
			PeriodUuid = prefix + periodUuid + suffix;

			DataUuid = DataUuid.ToUpper();
			ConfigurationUuid = ConfigurationUuid.ToUpper();
			PeriodUuid = PeriodUuid.ToUpper();

		}

	}
}
