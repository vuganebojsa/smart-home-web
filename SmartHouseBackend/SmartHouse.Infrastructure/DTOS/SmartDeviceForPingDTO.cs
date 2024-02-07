using SmartHouse.Core.Model;
using SmartHouse.Core.Model.SmartHomeDevices;

namespace SmartHouse.Infrastructure.DTOS
{
    public class SmartDeviceForPingDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool? IsOn { get; set; }

        public string? TypeOfPowerSupply { get; set; }
        public double? PowerUsage { get; set; }

        public int? EWNumberOfConnections { get; set; }
        public double? EWPower { get; set; }
        public double? EWPercentageOfCharge { get; set; }

        public double? HBBatterySize { get; set; }
        public double? HBOccupationLevel { get; set; }

        public List<SolarPanelDTO>? SPSPanels { get; set; }
        public double? LLuminosity { get; set; }
        public bool? VGIsPublic { get; set; }
        public List<string>? VGValidLicensePlates { get; set; }
        public double? ACMinTemperature { get; set; }
        public double? ACMaxTemperature { get; set; }
        public List<Mode>? ACModes { get; set; }
        public Mode? ACCurrentMode { get; set; }

        public Cycle? WMCurrentCycle { get; set; }
        public List<CycleDTO>? WMSupportedCycles { get; set; }
        public string? SmartDeviceType { get; set; }

        public double? ASRoomTemperature { get; set; }
        public double ? ASRoomHumidity { get; set; }

        public bool? SSIsSpecialMode { get; set; }

        public TimeSpan? SSStartSprinkle { get; set; }
        public TimeSpan? SSEndSprinkle { get; set; }

        public List<string>? SSActiveDays { get; set; } = new();
    }
}
