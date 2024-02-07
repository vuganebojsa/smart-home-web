using FluentResults;
using SmartHouse.Core.Messages;
using SmartHouse.Core.Model;
using SmartHouse.Extensions;
using SmartHouse.Infrastructure.DTOS;
using SmartHouse.Infrastructure.InfluxDTOs;
using SmartHouse.Infrastructure.Interfaces.Repositories;
using SmartHouse.Infrastructure.Interfaces.Services;
using SmartHouse.Infrastructure.MqttDTOs;

namespace SmartHouse.Services
{
    public class ReportService : IReportService
    {
        private readonly IInfluxDbService _influxService;
        private readonly ISmartDeviceRepository _smartDeviceRepository;
        private readonly IRepositoryBase<User> _userRepository;
        private readonly ISmartPropertyRepository _smartPropertyRepository;

        public ReportService(IInfluxDbService influxService,
           ISmartDeviceRepository smartDeviceRepository,
            IRepositoryBase<User> userRepository,
            ISmartPropertyRepository smartPropertyRepository)
        {
            _influxService = influxService;
            _smartDeviceRepository = smartDeviceRepository;
            _userRepository = userRepository;
            _smartPropertyRepository = smartPropertyRepository;

        }
        public async Task<Result<List<SPSActionsDTO>>> GetSolarPanelSystemHistory(Guid spsId, string username, string from, string to, Guid userId)
        {
            var sps = await _smartDeviceRepository.FindSingleByCondition(sps => sps.Id == spsId && sps.SmartProperty.User.Id == userId);
            if (sps == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            string userUsername = "";
            if (username.Trim() != "")
            {
                var user = await _userRepository.FindSingleByCondition(user => user.UserName.ToLower().Contains(username.ToLower()));
                if (user != null) { userUsername = user.UserName; }
                else return ResultExtensions.FailNotFound(ErrorMessages.NotFound("user", "username"));
            }
            var flux = "";
            if (from.Trim() != "" && to.Trim() != "")
            {
                DateTime today = DateTime.Now;
                DateTime parsedStartDate = DateTime.Parse(from);
                DateTime parsedEndDate = DateTime.Parse(to);

                var start = today - parsedStartDate;
                var end = parsedEndDate - today;
                int startSeconds = (int)(start.TotalSeconds);
                int endSeconds = (int)end.TotalSeconds;
                if (startSeconds < 0)
                {
                    return ResultExtensions.FailBadRequest("Range From should be before current time.");
                }
                flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"deviceonoff\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + spsId + "\")\n";


            }
            else if (from.Trim() != "")
            {
                DateTime today = DateTime.Now;
                DateTime parsedStartDate = DateTime.Parse(from);
                var start = today - parsedStartDate;

                int startHours = (int)(start.TotalHours);
                if (startHours < 0)
                {
                    return ResultExtensions.FailBadRequest("Range From should be before current time.");
                }
                flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startHours + "h)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"deviceonoff\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + spsId + "\")\n";
            }
            else if (to.Trim() != "")
            {
                DateTime today = DateTime.Now;
                DateTime parsedEndDate = DateTime.Parse(to);
                var end = parsedEndDate - today;
                int endSeconds = (int)end.TotalSeconds;
                flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"deviceonoff\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + spsId + "\")\n";
            }
            else
            {
                DateTime today = DateTime.Now;
                DateTime plus2Hours = DateTime.Now.AddHours(2);
                var end = plus2Hours - today;
                int endSeconds = (int)end.TotalSeconds;

                flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: 0, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"deviceonoff\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + spsId + "\")\n";
            }

            if (userUsername != "")
            {
                flux += "  |> filter(fn: (r) => r[\"username\"] == \"" + userUsername + "\")\n";
            }
            var result = await _influxService.QueryAsync(async query =>
            {

                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                    {

                        var tagValues = record.Values;
                        var userName = tagValues["username"].ToString();

                        return new SPSActionsDTO(double.Parse(record.GetValue().ToString()), DateTime.Parse(record.GetTime().ToString()), userName);
                    }
                       )).ToList();
            });
            return Result.Ok(result);
        }

        public async Task<Result<List<LuminosityDataDTO>>> GetLampLuminosityHistory(Guid id, string startDate, string endDate, Guid userId)
        {
            var lamp = await _smartDeviceRepository.FindSingleByCondition(lamp => lamp.Id == id && lamp.SmartProperty.User.Id == userId);
            if (lamp == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            var results = await _influxService.QueryAsync(async query =>
            {
                DateTime today = DateTime.Now;
                DateTime parsedStartDate = DateTime.Parse(startDate);
                DateTime parsedEndDate = DateTime.Parse(endDate);

                var start = today - parsedStartDate;
                var end = parsedEndDate - today;
                int startSeconds = (int)(start.TotalSeconds);
                int endSeconds = (int)end.TotalSeconds;
                int totalSeconds = endSeconds + startSeconds;
                string meanPeriod = "1";
                if (totalSeconds >= 24000000)
                {
                    meanPeriod = "90"; ;
                }
                else if (totalSeconds >= 600000)
                {
                    meanPeriod = "45";
                }
                else if (totalSeconds >= 85000)
                {
                    meanPeriod = "15";
                }
                else if (totalSeconds >= 42000)
                {
                    meanPeriod = "7"; ;
                }
                else if (totalSeconds >= 21000)
                {
                    meanPeriod = "5"; ;
                }



                var flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"luminosity\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n" +
                            "  |> aggregateWindow(every: " + meanPeriod + "m, fn: mean, createEmpty: false)\n";
                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new LuminosityDataDTO
                        {
                            Timestamp = record.GetTime().ToString(),
                            Luminosity = double.Parse(record.GetValue().ToString())
                        })).ToList();
            });

            return results;

        }

        public async Task<Result<List<GateEventInfoDTO>>> GetGateEventHistory(Guid id, string startDate, string endDate, string licencePlate, Guid userId)
        {
            var gate = await _smartDeviceRepository.FindSingleByCondition(gate => gate.Id == id && gate.SmartProperty.User.Id == userId);
            if (gate == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            var results = await _influxService.QueryAsync(async query =>
            {
                DateTime today = DateTime.Now;
                DateTime parsedStartDate = DateTime.Parse(startDate);
                DateTime parsedEndDate = DateTime.Parse(endDate);

                var start = today - parsedStartDate;
                var end = parsedEndDate - today;
                int startSeconds = (int)(start.TotalSeconds);
                int endSeconds = (int)end.TotalSeconds;

                var flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"event\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n";
                if (licencePlate != null)
                {
                    flux = "from(bucket: \"smarthome\")\n" +
                           "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                           "  |> filter(fn: (r) => r[\"_measurement\"] == \"event\")\n" +
                           "  |> filter(fn: (r) => r[\"licencePlate\"] == \"" + licencePlate + "\")\n" +
                           "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n";
                }


                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new GateEventInfoDTO
                        {
                            Timestamp = record.GetTime().ToString(),
                            Action = int.Parse(record.GetValue().ToString()),
                            licencePlate = record.GetValueByKey("licencePlate")?.ToString()
                        })).ToList();
            });

            return results;
        }

        public async Task<Result<List<SprinklerEventInfoDTO>>> GetSprinklerEventHistory(Guid id, string startDate, string endDate, string username, Guid userId)
        {
            var sprinkler = await _smartDeviceRepository.FindSingleByCondition(sprinkler => sprinkler.Id == id && sprinkler.SmartProperty.User.Id == userId);
            if (sprinkler == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            var results = await _influxService.QueryAsync(async query =>
            {
                DateTime today = DateTime.Now;
                DateTime parsedStartDate = DateTime.Parse(startDate);
                DateTime parsedEndDate = DateTime.Parse(endDate);

                var start = today - parsedStartDate;
                var end = parsedEndDate - today;
                int startSeconds = (int)(start.TotalSeconds);
                int endSeconds = (int)end.TotalSeconds;

                var flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"sprinkleEvent\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n";
                if (username != null)
                {

                    flux = "from(bucket: \"smarthome\")\n" +
                           "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                           "  |> filter(fn: (r) => r[\"_measurement\"] == \"sprinkleEvent\")\n" +
                           "  |> filter(fn: (r) => r[\"username\"] == \"" + username + "\")\n" +
                           "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n";
                }


                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>

                        new SprinklerEventInfoDTO
                        {
                            Timestamp = record.GetTime().ToString(),
                            Action = record.GetValueByKey("actionNumber")?.ToString(),
                            Username = record.GetValueByKey("username")?.ToString(),
                            Value = record.GetValue().ToString()
                        })).ToList();

            });

            return results;
        }
        public async Task<Result<List<DeviceOnOffInfoDTO>>> GetDeviceOnOffHistory(Guid id, string startDate, string endDate, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == id && device.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            var results = await _influxService.QueryAsync(async query =>
            {
                DateTime today = DateTime.Now;
                DateTime parsedStartDate = DateTime.Parse(startDate);
                DateTime parsedEndDate = DateTime.Parse(endDate);

                var start = today - parsedStartDate;
                var end = parsedEndDate - today;
                int startSeconds = (int)(start.TotalSeconds);
                int endSeconds = (int)end.TotalSeconds;



                var flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"status\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n";
                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new DeviceOnOffInfoDTO
                        {
                            Timestamp = record.GetTime().ToString(),
                            isOn = int.Parse(record.GetValue().ToString())
                        })).ToList();
            });

            return results;
        }

        public async Task<Result<OnlineOfflineHistoryDTO>> GetDeviceOnlineOfflineHistory(Guid id, string startDate, string endDate, Guid userId)
        {
            var device = await _smartDeviceRepository.FindSingleByCondition(device => device.Id == id && device.SmartProperty.User.Id == userId);
            if (device == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id"));
            }
            bool twoZeros = false;
            int totalTimeOnline = 0;
            int totalTimeOffline = 0;
            double onlinePercentage = 0;

            DateTime today = DateTime.Now;
            DateTime parsedStartDate = DateTime.Parse(startDate);

            DateTime parsedEndDate = DateTime.Parse(endDate);

            if (parsedEndDate > today)
            {
                parsedEndDate = today;
            }
            var start = today - parsedStartDate;
            var end = parsedEndDate - today;
            int startSeconds = (int)(start.TotalSeconds);
            int endSeconds = (int)end.TotalSeconds;
            int totalSeconds = startSeconds + endSeconds;
            var results = await _influxService.QueryAsync(async query =>
            {


                var flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"onlineStatus\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n";
                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new DeviceOnlineOfflineInfoDTO
                        {
                            Timestamp = record.GetTime().ToString(),
                            isOnline = int.Parse(record.GetValue().ToString())
                        })).ToList();
            });


            Dictionary<string, int> myHashMap = new Dictionary<string, int>();
            Dictionary<string, int> myOfflineMap = new Dictionary<string, int>();
            DateTime myOnlineDate = today;
            DateTime myOfflineDate;
            string formattedFirstDate = "";
            string formattedLastDate = "";
            string formattedDateString = today.ToString("yyyy-MM-ddTHH:mm:ssZ");
            if (totalSeconds <= 86400)
            {
                foreach (DeviceOnlineOfflineInfoDTO record in results)
                {


                    if (record.isOnline == 1)
                    {
                        twoZeros = false;
                        myOnlineDate = DateTime.ParseExact(record.Timestamp, "yyyy-MM-ddTHH:mm:ssZ", null);
                        myOnlineDate = myOnlineDate.AddHours(-1);
                        formattedFirstDate = myOnlineDate.ToString("yyyy-MM-ddTHH:mm:ssZ");

                    }
                    if (record.isOnline == 0 && !twoZeros)
                    {
                        twoZeros = true;

                        myOfflineDate = DateTime.ParseExact(record.Timestamp, "yyyy-MM-ddTHH:mm:ssZ", null);
                        myOfflineDate = myOfflineDate.AddHours(-1);
                        if (myOnlineDate > myOfflineDate)
                        {
                            continue;
                        }
                        formattedLastDate = myOfflineDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
                        if (myOfflineDate.Hour == myOnlineDate.Hour)
                        {
                            TimeSpan timeDifference = myOfflineDate - myOnlineDate;
                            int totalSecondsPassedBetween = (int)timeDifference.TotalSeconds;
                            string hour = formattedFirstDate.Split(":")[0];

                            if (myHashMap.ContainsKey(hour))
                            {

                                myHashMap[hour] += totalSecondsPassedBetween;
                            }
                            else
                            {
                                myHashMap[hour] = totalSecondsPassedBetween;
                            }

                        }
                        else
                        {
                            DateTime roundedStartTime = new DateTime(myOnlineDate.Year, myOnlineDate.Month, myOnlineDate.Day, myOnlineDate.Hour, 0, 0);
                            DateTime nextHour = roundedStartTime.AddHours(1);

                            TimeSpan timeDifference = nextHour - myOnlineDate;
                            int totalSecondsPassedBetween = (int)timeDifference.TotalSeconds;
                            string hour = formattedFirstDate.Split(":")[0];
                            if (myHashMap.ContainsKey(hour))
                            {
                                myHashMap[hour] += totalSecondsPassedBetween;
                            }
                            else
                            {

                                myHashMap[hour] = totalSecondsPassedBetween;
                            }
                            DateTime myOfflineDateHours = new DateTime(myOfflineDate.Year, myOfflineDate.Month, myOfflineDate.Day, myOfflineDate.Hour, 0, 0);



                            DateTime currentStartHours = new DateTime(myOnlineDate.Year, myOnlineDate.Month, myOnlineDate.Day, myOnlineDate.Hour, 0, 0);


                            for (DateTime currentHour = currentStartHours; currentHour < myOfflineDateHours; currentHour = currentHour.AddHours(1))
                            {

                                string formattedCurrentHour = currentHour.ToString("yyyy-MM-ddTHH:mm:ssZ").Split(":")[0];
                                totalSecondsPassedBetween = 3600;
                                if (myHashMap.ContainsKey(formattedCurrentHour))
                                {


                                }
                                else
                                {
                                    myHashMap[formattedCurrentHour] = totalSecondsPassedBetween;
                                }


                            }
                            DateTime roundedEndTime = new DateTime(myOfflineDate.Year, myOfflineDate.Month, myOfflineDate.Day, myOfflineDate.Hour, 0, 0);

                            TimeSpan timeDifferenceEnd = myOfflineDate - roundedEndTime;
                            totalSecondsPassedBetween = (int)timeDifferenceEnd.TotalSeconds;
                            string hourEnd = formattedLastDate.Split(":")[0];

                            if (myHashMap.ContainsKey(hourEnd))
                            {
                                myHashMap[hourEnd] += totalSecondsPassedBetween;
                            }
                            else
                            {
                                myHashMap[hourEnd] = totalSecondsPassedBetween;
                            }

                        }
                    }

                }

            }
            else
            {

                foreach (DeviceOnlineOfflineInfoDTO record in results)
                {


                    if (record.isOnline == 1)
                    {
                        twoZeros = false;
                        myOnlineDate = DateTime.ParseExact(record.Timestamp, "yyyy-MM-ddTHH:mm:ssZ", null);
                        formattedFirstDate = myOnlineDate.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");


                    }
                    if (record.isOnline == 0 && !twoZeros)
                    {
                        twoZeros = true;

                        myOfflineDate = DateTime.ParseExact(record.Timestamp, "yyyy-MM-ddTHH:mm:ssZ", null);
                        formattedLastDate = myOfflineDate.AddHours(-1).ToString("yyyy-MM-ddTHH:mm:ssZ");
                        if (myOnlineDate > myOfflineDate) { continue; }
                        if (myOfflineDate.Date == myOnlineDate.Date)
                        {

                            TimeSpan timeDifference = myOfflineDate - myOnlineDate;
                            int totalSecondsPassedBetween = (int)timeDifference.TotalSeconds;
                            string day = formattedFirstDate.Split("T")[0];
                            if (myHashMap.ContainsKey(day))
                            {

                                myHashMap[day] += totalSecondsPassedBetween;
                            }
                            else
                            {

                                myHashMap[day] = totalSecondsPassedBetween;
                            }

                        }

                    }
                }


            }




            if (totalSeconds <= 86400)
            {
                DateTime endDateHours = new DateTime(parsedEndDate.Year, parsedEndDate.Month, parsedEndDate.Day, parsedEndDate.Hour, 0, 0);
                DateTime currentStartHours = new DateTime(parsedStartDate.Year, parsedStartDate.Month, parsedStartDate.Day, parsedStartDate.Hour, 0, 0);

                for (DateTime currentHour = currentStartHours; currentHour < endDateHours; currentHour = currentHour.AddHours(1))
                {
                    string formattedCurrentHour = currentHour.ToString("yyyy-MM-ddTHH:mm:ssZ").Split(":")[0];

                    if (!myHashMap.ContainsKey(formattedCurrentHour))
                    {


                        myHashMap[formattedCurrentHour] = 0;
                        myOfflineMap[formattedCurrentHour] = 3600;
                    }
                    else
                    {
                        myOfflineMap[formattedCurrentHour] = 3600 - myHashMap[formattedCurrentHour];
                    }



                }

            }
            else
            {
                DateTime EndDateHours = new DateTime(parsedEndDate.Year, parsedEndDate.Month, parsedEndDate.Day, 0, 0, 0);
                DateTime currentStartDate = new DateTime(parsedStartDate.Year, parsedStartDate.Month, parsedStartDate.Day, 0, 0, 0);

                for (DateTime currentHour = currentStartDate; currentHour <= EndDateHours; currentHour = currentHour.AddDays(1))
                {
                    string formattedCurrentHour = currentHour.ToString("yyyy-MM-ddTHH:mm:ssZ").Split("T")[0];


                    if (!myHashMap.ContainsKey(formattedCurrentHour))
                    {


                        myHashMap[formattedCurrentHour] = 0;
                        myOfflineMap[formattedCurrentHour] = 86400;
                    }
                    else
                    {
                        myOfflineMap[formattedCurrentHour] = 86400 - myHashMap[formattedCurrentHour];
                    }



                }

            }

            foreach (var item in myHashMap)
            {
                if (item.Key == "")
                {
                    myHashMap.Remove(item.Key);

                }

            }
            foreach (var pair in myHashMap)
            {
                totalTimeOnline += pair.Value;

            }
            totalTimeOffline = totalSeconds - totalTimeOnline;
            onlinePercentage = (double)totalTimeOnline / startSeconds;

            return new OnlineOfflineHistoryDTO
            {
                TotalTimeOffline = totalTimeOffline,
                TotalTimeOnline = totalTimeOnline,
                OnlineMap = myHashMap,
                OfflineMap = myOfflineMap,
                PercentageOnline = onlinePercentage,
            };
        }

        public async Task<Result<List<GatePublicPrivateInfoDTO>>> GetGatePublicPrivateHistory(Guid id, string startDate, string endDate, Guid userId)
        {
            var gate = await _smartDeviceRepository.FindSingleByCondition(gate => gate.Id == id && gate.SmartProperty.User.Id == userId);
            if (gate == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }
            var results = await _influxService.QueryAsync(async query =>
            {
                DateTime today = DateTime.Now;
                DateTime parsedStartDate = DateTime.Parse(startDate);
                DateTime parsedEndDate = DateTime.Parse(endDate);
                string meanPeriod = "1";

                var start = today - parsedStartDate;
                var end = parsedEndDate - today;
                int startSeconds = (int)(start.TotalSeconds);
                int endSeconds = (int)end.TotalSeconds;




                var flux = "from(bucket: \"smarthome\")\n" +
                            "  |> range(start: -" + startSeconds + "s, stop: " + endSeconds + "s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"gateStatus\")\n" +
                            "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + id + "\")\n";

                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new GatePublicPrivateInfoDTO
                        {
                            Timestamp = record.GetTime().ToString(),
                            isPublic = int.Parse(record.GetValue().ToString())
                        })).ToList();
            });

            return results;
        }



        public async Task<Result<List<PropertyConsumptionDTO>>> GetEnergyConsumption(Guid smartPropertyId, TotalTimePeriod totalTimePeriod, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }
            var property = await _smartPropertyRepository.FindById(smartPropertyId);
            if (property == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id")); }
            var timePeriod = GetTimePeriod(totalTimePeriod);
            var getMeanFromTimePeriod = GetTimePeriod(totalTimePeriod);
            var flux =
              "from(bucket: \"smarthome\")\n" +
              "  |> range(start: -" + timePeriod + "h)\n" +
              "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
              "  |> filter(fn: (r) => r[\"_field\"] == \"totalConsumedByProperty\")\n" +
              "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + smartPropertyId + "\")\n" +
              "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: mean, createEmpty: false)\n";
            var fluxMax =
              "from(bucket: \"smarthome\")\n" +
              "  |> range(start: -" + timePeriod + "h)\n" +
              "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
              "  |> filter(fn: (r) => r[\"_field\"] == \"totalConsumedByProperty\")\n" +
              "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + smartPropertyId + "\")\n" +
              "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: max, createEmpty: false)\n";

            var fluxMin =
              "from(bucket: \"smarthome\")\n" +
              "  |> range(start: -" + timePeriod + "h)\n" +
              "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
              "  |> filter(fn: (r) => r[\"_field\"] == \"totalConsumedByProperty\")\n" +
              "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + smartPropertyId + "\")\n" +
              "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: min, createEmpty: false)\n";
            var results = await _influxService.QueryAsync(async query =>
            {

                var tables = await query.QueryAsync(flux, "FTN");
                var meanList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new PropertyConsumptionDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            Consumed = double.Parse(record.GetValue().ToString())
                        })).ToList();
                tables = await query.QueryAsync(fluxMax, "FTN");
                var maxList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new PropertyConsumptionDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            Consumed = double.Parse(record.GetValue().ToString())
                        })).ToList();
                tables = await query.QueryAsync(fluxMin, "FTN");
                var minList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new PropertyConsumptionDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            Consumed = double.Parse(record.GetValue().ToString())
                        })).ToList();
                var distinctList = meanList
                .Concat(maxList)
                .Concat(minList)
                .GroupBy(dto => new { dto.Timestamp, dto.Consumed })
                .Select(group => group.First())
                .OrderBy(dto => dto.Timestamp)
                .ToList();
                return distinctList;
            });
            return Result.Ok(results);
        }

        private string GetMeanFromTimePeriod(TotalTimePeriod totalTimePeriod)
        {
            switch (totalTimePeriod)
            {
                case TotalTimePeriod.SIX_HOURS:
                    {
                        return "5";

                    }
                case TotalTimePeriod.TWELVE_HOURS:
                    {
                        return "10";
                    }
                case TotalTimePeriod.TWENTY_FOUR_HOURS:
                    {
                        return "15";
                    }
                case TotalTimePeriod.LAST_WEEK:
                    {
                        return "60";
                    }
                case TotalTimePeriod.LAST_MONTH:
                    {
                        return "120";
                    }
                default:
                    {
                        break;

                    }

            }

            return "6";
        }

        private static string GetTimePeriod(TotalTimePeriod totalTimePeriod)
        {
            switch (totalTimePeriod)
            {
                case TotalTimePeriod.SIX_HOURS:
                    {
                        return "6";

                    }
                case TotalTimePeriod.TWELVE_HOURS:
                    {
                        return "12";
                    }
                case TotalTimePeriod.TWENTY_FOUR_HOURS:
                    {
                        return "24";
                    }
                case TotalTimePeriod.LAST_WEEK:
                    {
                        return "168";
                    }
                case TotalTimePeriod.LAST_MONTH:
                    {
                        return "720";
                    }
                default:
                    {
                        break;

                    }

            }

            return "6";
        }

        public async Task<Result<List<PropertyConsumptionDTO>>> GetPropertyEnergyConsumption(Guid smartPropertyId, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }
            var property = await _smartPropertyRepository.FindById(smartPropertyId);
            if (property == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id")); }
            var fluxMean = "from(bucket: \"smarthome\")\n" +
                           "  |> range(start: -60m)\n" +
                           "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
                           "  |> filter(fn: (r) => r[\"_field\"] == \"totalConsumedByProperty\")\n" +
                          "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + smartPropertyId + "\")\n" +
                          "  |> aggregateWindow(every: 3m, fn: mean, createEmpty: false)\n";
            var fluxMax =
             "from(bucket: \"smarthome\")\n" +
             "  |> range(start: -60m)\n" +
             "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
             "  |> filter(fn: (r) => r[\"_field\"] == \"totalConsumedByProperty\")\n" +
             "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + smartPropertyId + "\")\n" +
             "  |> aggregateWindow(every: 3m, fn: max, createEmpty: false)\n";
            var fluxMin =
           "from(bucket: \"smarthome\")\n" +
           "  |> range(start: -60m)\n" +
           "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
           "  |> filter(fn: (r) => r[\"_field\"] == \"totalConsumedByProperty\")\n" +
           "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + smartPropertyId + "\")\n" +
           "  |> aggregateWindow(every: 3m, fn: min, createEmpty: false)\n";

            var results = await _influxService.QueryAsync(async query =>
            {


                var tables = await query.QueryAsync(fluxMean, "FTN");
                var meanList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new PropertyConsumptionDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            Consumed = double.Parse(record.GetValue().ToString())
                        })).ToList();
                tables = await query.QueryAsync(fluxMax, "FTN");
                var maxList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new PropertyConsumptionDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            Consumed = double.Parse(record.GetValue().ToString())
                        })).ToList();
                tables = await query.QueryAsync(fluxMin, "FTN");
                var minList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new PropertyConsumptionDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            Consumed = double.Parse(record.GetValue().ToString())
                        })).ToList();
                var distinctList = meanList
                .Concat(maxList)
                .Concat(minList)
                .GroupBy(dto => new { dto.Timestamp, dto.Consumed })
                .Select(group => group.First())
                .OrderBy(dto => dto.Timestamp)
                .ToList();
                return distinctList;
            });
            return Result.Ok(results);
        }


        public async Task<Result<List<TemperatureDTO>>> GetTemperatureData(Guid smartDeviceId, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }

            var device = await _smartDeviceRepository.FindById(smartDeviceId);
            if (device == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            var fluxMean = "from(bucket: \"smarthome\")\n" +
                           "  |> range(start: -60m)\n" +
                           "  |> filter(fn: (r) => r[\"_measurement\"] == \"temperature\")\n" +
                          "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + smartDeviceId + "\")\n" +
                          "  |> aggregateWindow(every: 3m, fn: mean, createEmpty: false)\n";

            var results = await _influxService.QueryAsync(async query =>
            {


                var tables = await query.QueryAsync(fluxMean, "FTN");
                var meanList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new TemperatureDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            RoomTemperature = double.Parse(record.GetValue().ToString())
                        })).ToList();

                return meanList;

            });

            return Result.Ok(results);
        }


        public async Task<Result<List<TemperatureDTO>>> GetTemperatureTimePeriod(Guid smartDeviceId, TotalTimePeriod totalTimePeriod, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }

            var device = await _smartDeviceRepository.FindById(smartDeviceId);
            if (device == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            var timePeriod = GetTimePeriod(totalTimePeriod);
            var getMeanFromTimePeriod = GetTimePeriod(totalTimePeriod);



            var fluxMean = "from(bucket: \"smarthome\")\n" +
                           "  |> range(start: -" + timePeriod + "h)\n" +
                           "  |> filter(fn: (r) => r[\"_measurement\"] == \"temperature\")\n" +
                          "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + smartDeviceId + "\")\n" +
                          "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: mean, createEmpty: false)\n";

            var results = await _influxService.QueryAsync(async query =>
            {


                var tables = await query.QueryAsync(fluxMean, "FTN");
                var meanList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new TemperatureDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            RoomTemperature = double.Parse(record.GetValue().ToString())
                        })).ToList();


                return meanList;

            });

            return Result.Ok(results);
        }

        public async Task<Result<List<TemperatureDTO>>> GetTemperatureFromTo(Guid smartDeviceId, string from, string to, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }

            var device = await _smartDeviceRepository.FindById(smartDeviceId);
            if (device == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            if (DateTime.TryParse(from, out DateTime parsedStartDate) && DateTime.TryParse(to, out DateTime parsedEndDate))
            {
                TimeSpan difference = parsedEndDate - parsedStartDate;
                bool isMonthApart = difference.TotalDays >= 30;
                if (!isMonthApart)
                {
                    var formattedStartDate = parsedStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    var formattedEndDate = parsedEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    var fluxMean = "from(bucket: \"smarthome\")\n" +
                                   "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
                                   "  |> filter(fn: (r) => r[\"_measurement\"] == \"temperature\")\n" +
                                  "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + smartDeviceId + "\")\n" +
                                  "  |> aggregateWindow(every: 5m, fn: mean, createEmpty: false)\n";

                    var results = await _influxService.QueryAsync(async query =>
                    {


                        var tables = await query.QueryAsync(fluxMean, "FTN");
                        var meanList = tables.SelectMany(table =>
                            table.Records.Select(record =>
                                new TemperatureDTO
                                {
                                    Timestamp = DateTime.Parse(record.GetTime().ToString()),
                                    RoomTemperature = double.Parse(record.GetValue().ToString())
                                })).ToList();


                        return meanList;

                    });

                    return Result.Ok(results);
                }
                else
                {
                    return ResultExtensions.FailBadRequest("The difference between dates is more than a month.");
                }
            }
            else
            {
                return ResultExtensions.FailBadRequest("Date Range not in correct format.");
            }



        }

        public async Task<Result<List<PropertyConsumptionDTO>>> GetEnergyConsumptionFromTo(Guid smartPropertyId, string from, string to, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }
            var property = await _smartPropertyRepository.FindById(smartPropertyId);
            if (property == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id")); }

            if (DateTime.TryParse(from, out DateTime parsedStartDate) && DateTime.TryParse(to, out DateTime parsedEndDate))
            {

                TimeSpan difference = parsedEndDate - parsedStartDate;
                bool isMonthApart = difference.TotalDays >= 30;
                if (!isMonthApart)
                {
                    var formattedStartDate = parsedStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    var formattedEndDate = parsedEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");


                    var flux = "from(bucket: \"smarthome\")\n" +
                          "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
                          "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
                          "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + smartPropertyId + "\")\n" +
                           "  |> aggregateWindow(every: 5m, fn: mean, createEmpty: false)\n";

                    var results = await _influxService.QueryAsync(async query =>
                    {

                        var tables = await query.QueryAsync(flux, "FTN");
                        return tables.SelectMany(table =>
                            table.Records.Select(record =>
                                new PropertyConsumptionDTO
                                {
                                    Timestamp = DateTime.Parse(record.GetTime().ToString()),
                                    Consumed = double.Parse(record.GetValue().ToString())
                                })).ToList();
                    });
                    return Result.Ok(results);
                }
                else
                {
                    return ResultExtensions.FailBadRequest("The difference between dates is more than a month.");
                }
            }
            else
            {
                return ResultExtensions.FailBadRequest("Date Range not in correct format.");
            }


        }


        public async Task<Result<List<HumidityDTO>>> GetHumidityTimePeriod(Guid smartDeviceId, TotalTimePeriod totalTimePeriod, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }

            var device = await _smartDeviceRepository.FindById(smartDeviceId);
            if (device == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            var timePeriod = GetTimePeriod(totalTimePeriod);
            var getMeanFromTimePeriod = GetTimePeriod(totalTimePeriod);


            var fluxMean = "from(bucket: \"smarthome\")\n" +
                           "  |> range(start: -" + timePeriod + "h)\n" +
                           "  |> filter(fn: (r) => r[\"_measurement\"] == \"humidity\")\n" +
                          "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + smartDeviceId + "\")\n" +
                          "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: mean, createEmpty: false)\n";

            var results = await _influxService.QueryAsync(async query =>
            {


                var tables = await query.QueryAsync(fluxMean, "FTN");
                var meanList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new HumidityDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            RoomHumidity = double.Parse(record.GetValue().ToString())
                        })).ToList();


                return meanList;

            });

            return Result.Ok(results);
        }

        public async Task<Result<List<HumidityDTO>>> GetHumidityFromTo(Guid smartDeviceId, string from, string to, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }

            var device = await _smartDeviceRepository.FindById(smartDeviceId);
            if (device == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            if (DateTime.TryParse(from, out DateTime parsedStartDate) && DateTime.TryParse(to, out DateTime parsedEndDate))
            {
                TimeSpan difference = parsedEndDate - parsedStartDate;
                bool isMonthApart = difference.TotalDays >= 30;
                if (!isMonthApart)
                {
                    var formattedStartDate = parsedStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    var formattedEndDate = parsedEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                    var fluxMean = "from(bucket: \"smarthome\")\n" +
                                   "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
                                   "  |> filter(fn: (r) => r[\"_measurement\"] == \"humidity\")\n" +
                                  "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + smartDeviceId + "\")\n" +
                                  "  |> aggregateWindow(every: 5m, fn: mean, createEmpty: false)\n";

                    var results = await _influxService.QueryAsync(async query =>
                    {


                        var tables = await query.QueryAsync(fluxMean, "FTN");
                        var meanList = tables.SelectMany(table =>
                            table.Records.Select(record =>
                                new HumidityDTO
                                {
                                    Timestamp = DateTime.Parse(record.GetTime().ToString()),
                                    RoomHumidity = double.Parse(record.GetValue().ToString())
                                })).ToList();


                        return meanList;

                    });

                    return Result.Ok(results);
                }
                else
                {
                    return ResultExtensions.FailBadRequest("The difference between dates is more than a month.");
                }
            }
            else
            {
                return ResultExtensions.FailBadRequest("Date Range not in correct format.");
            }

        }


        public async Task<Result<List<HumidityDTO>>> GetHumidityData(Guid smartDeviceId, Guid userId)
        {
            var user = await _userRepository.FindById(userId);
            if (user == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId()); }

            var device = await _smartDeviceRepository.FindById(smartDeviceId);
            if (device == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            var fluxMean = "from(bucket: \"smarthome\")\n" +
                           "  |> range(start: -60m)\n" +
                           "  |> filter(fn: (r) => r[\"_measurement\"] == \"humidity\")\n" +
                          "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + smartDeviceId + "\")\n" +
                          "  |> aggregateWindow(every: 3m, fn: mean, createEmpty: false)\n";

            var results = await _influxService.QueryAsync(async query =>
            {


                var tables = await query.QueryAsync(fluxMean, "FTN");
                var meanList = tables.SelectMany(table =>
                    table.Records.Select(record =>
                        new HumidityDTO
                        {
                            Timestamp = DateTime.Parse(record.GetTime().ToString()),
                            RoomHumidity = double.Parse(record.GetValue().ToString())
                        })).ToList();

                return meanList;

            });

            return Result.Ok(results);
        }

        public async Task<Result<List<VehicleChargerActionsDTO>>> GetVehicleChargerHistory(Guid id, Guid userId)
        {
            var charger = await _smartDeviceRepository.FindSingleByCondition(charger => charger.Id == id && charger.SmartProperty.User.Id == userId);
            if (charger == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            var flux = "from(bucket: \"smarthome\")\n" +
                          "  |> range(start: 0)\n" +
                          "  |> filter(fn: (r) => r[\"_measurement\"] == \"vehiclecharger\")\n" +
                          "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + charger.Id + "\")\n";

            var result = await _influxService.QueryAsync(async query =>
            {

                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                    {

                        var tagValues = record.Values;
                        var userName = tagValues["username"].ToString();

                        return new VehicleChargerActionsDTO(double.Parse(record.GetValue().ToString()), DateTime.Parse(record.GetTime().ToString()), userName);
                    }
                       )).ToList();
            });
            return Result.Ok(result);
        }

        public async Task<Result<List<VehicleChargerAllActionsDTO>>> GetAllActionsByVehicleChargerInRange(Guid id, string from, string to, Guid userId, string executer)
        {
            var charger = await _smartDeviceRepository.FindSingleByCondition(charger => charger.Id == id && charger.SmartProperty.User.Id == userId);
            if (charger == null) { return ResultExtensions.FailNotFound(ErrorMessages.NotFound("device", "id")); }

            if (!DateTime.TryParse(from, out _) || !DateTime.TryParse(to, out _))
            {
                return ResultExtensions.FailBadRequest("Invalid date format for 'from' or 'to' parameters.");
            }
            DateTime parsedStartDate = DateTime.Parse(from);
            DateTime parsedEndDate = DateTime.Parse(to);
            if (parsedStartDate >= parsedEndDate)
            {
                return ResultExtensions.FailBadRequest("Start Date must be before end date.");

            }
            if (parsedStartDate >= DateTime.Now)
            {
                return ResultExtensions.FailBadRequest("Start Date must be before current date.");

            }
            var formattedStartDate = parsedStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var formattedEndDate = parsedEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var result = await _influxService.QueryAsync(async query =>
            {
                var flux = "from(bucket: \"smarthome\")\n" +
                                      "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
                                      "  |> filter(fn: (r) => r[\"_measurement\"] == \"vehiclecharger\")\n" +
                                      "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + charger.Id + "\")\n";
                if (executer != "")
                {
                    flux += "  |> filter(fn: (r) => r[\"username\"] == \"" + executer + "\")\n";

                }
                var tables = await query.QueryAsync(flux, "FTN");
                var firstQuery = tables.SelectMany(table =>
                    table.Records.Select(record =>
                    {

                        var tagValues = record.Values;
                        var userName = tagValues["username"].ToString();

                        return new VehicleChargerAllActionsDTO(record.GetValue().ToString(), DateTime.Parse(record.GetTime().ToString()), userName, "Change max Charge Percentage");
                    }
                       )).ToList();
                flux = "from(bucket: \"smarthome\")\n" +
                        "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
                        "  |> filter(fn: (r) => r[\"_measurement\"] == \"vehiclecharging\")\n" +
                        "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + charger.Id + "\")\n" +
                        "  |> filter(fn: (r) => r[\"_field\"] == \"startChargeNeededKW\")\n";
                if (executer != "")
                {
                    flux += "  |> filter(fn: (r) => r[\"plate\"] == \"" + executer + "\")\n";

                }
                tables = await query.QueryAsync(flux, "FTN");

                var secondQuery = tables.SelectMany(table =>
                   table.Records.Select(record =>
                   {

                       var tagValues = record.Values;
                       var userName = tagValues["plate"].ToString();

                       return new VehicleChargerAllActionsDTO(record.GetValue().ToString(), DateTime.Parse(record.GetTime().ToString()), userName, "Start Charge");
                   }
                      )).ToList();
                flux = "from(bucket: \"smarthome\")\n" +
                        "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
                        "  |> filter(fn: (r) => r[\"_measurement\"] == \"vehiclecharging\")\n" +
                        "  |> filter(fn: (r) => r[\"deviceId\"] == \"" + charger.Id + "\")\n" +
                        "  |> filter(fn: (r) => r[\"_field\"] == \"endChargeConsumedKWH\")\n";
                if (executer != "")
                {
                    flux += "  |> filter(fn: (r) => r[\"plate\"] == \"" + executer + "\")\n";

                }
                tables = await query.QueryAsync(flux, "FTN");

                var thirdQuery = tables.SelectMany(table =>
                   table.Records.Select(record =>
                   {

                       var tagValues = record.Values;
                       var userName = tagValues["plate"].ToString();

                       return new VehicleChargerAllActionsDTO(record.GetValue().ToString(), DateTime.Parse(record.GetTime().ToString()), userName, "End Charge");
                   }
                      )).ToList();
                var combinedList = firstQuery.Concat(secondQuery).Concat(thirdQuery).ToList();

                return combinedList.OrderBy(item => item.TimeStamp).ToList();
            });


            return Result.Ok(result);
        }


        public async Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByCity(string cityName, TotalTimePeriod totalTimePeriod, Guid adminId, bool isConsumed)
        {
            var admin = await _userRepository.FindById(adminId);
            if (admin == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            if (admin.Role != Role.ADMIN && admin.Role != Role.SUPERADMIN)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            var timePeriod = GetTimePeriod(totalTimePeriod);
            var getMeanFromTimePeriod = GetTimePeriod(totalTimePeriod);
            var flux = "";
            if (isConsumed)
            {
                flux =
                  "from(bucket: \"smarthome\")\n" +
                  "  |> range(start: -" + timePeriod + "h)\n" +
                  "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
                  "  |> filter(fn: (r) => r[\"city\"] == \"" + cityName + "\")\n" +
                  "  |> group(columns: [\"city\"])\n" +
                  "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: sum, createEmpty: false)\n";
            }
            else
            {
                flux =
                 "from(bucket: \"smarthome\")\n" +
                 "  |> range(start: -" + timePeriod + "h)\n" +
                 "  |> filter(fn: (r) => r[\"_measurement\"] == \"electricenergy\")\n" +
                 "  |> filter(fn: (r) => r[\"city\"] == \"" + cityName + "\")\n" +
                 "  |> filter(fn: (r) => r[\"_field\"] == \"totalProduced\")\n" +
                 "  |> group(columns: [\"city\"])\n" +
                 "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: sum, createEmpty: false)\n";
            }
            var result = await _influxService.QueryAsync(async query =>
            {

                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                    {
                        return new EnergyConsumptionDTO(DateTime.Parse(record.GetTime().ToString()), double.Parse(record.GetValue().ToString()));
                    }
                       )).ToList();
            });
            return Result.Ok(result);

        }

        public async Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByCityInRange(string cityName, string from, string to, Guid adminId, bool isConsumed)
        {
            var admin = await _userRepository.FindById(adminId);
            if (admin == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            if (admin.Role != Role.ADMIN && admin.Role != Role.SUPERADMIN)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            if (!DateTime.TryParse(from, out _) || !DateTime.TryParse(to, out _))
            {
                return ResultExtensions.FailBadRequest("Invalid date format for 'from' or 'to' parameters.");
            }
            DateTime parsedStartDate = DateTime.Parse(from);
            DateTime parsedEndDate = DateTime.Parse(to);
            if (parsedStartDate >= parsedEndDate)
            {
                return ResultExtensions.FailBadRequest("Start Date must be before end date.");

            }
            if (parsedStartDate >= DateTime.Now)
            {
                return ResultExtensions.FailBadRequest("Start Date must be before current date.");

            }
            TimeSpan difference = parsedEndDate - parsedStartDate;
            bool isMonthApart = difference.TotalDays >= 31;
            if (isMonthApart)
            {
                return ResultExtensions.FailBadRequest("The difference between dates is more than a month.");

            }
            var formattedStartDate = parsedStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var formattedEndDate = parsedEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var flux = "";
            if (isConsumed)
            {
                flux =
              "from(bucket: \"smarthome\")\n" +
              "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
              "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
              "  |> filter(fn: (r) => r[\"city\"] == \"" + cityName + "\")\n" +
              "  |> group(columns: [\"city\"])\n" +
              "  |> aggregateWindow(every: 30m, fn: sum, createEmpty: false)\n";
            }
            else
            {
                flux =
             "from(bucket: \"smarthome\")\n" +
             "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
             "  |> filter(fn: (r) => r[\"_measurement\"] == \"electricenergy\")\n" +
             "  |> filter(fn: (r) => r[\"city\"] == \"" + cityName + "\")\n" +
             "  |> filter(fn: (r) => r[\"_field\"] == \"totalProduced\")\n" +
             "  |> group(columns: [\"city\"])\n" +
             "  |> aggregateWindow(every: 30m, fn: sum, createEmpty: false)\n";
            }
            var result = await _influxService.QueryAsync(async query =>
            {

                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                    {
                        return new EnergyConsumptionDTO(DateTime.Parse(record.GetTime().ToString()), double.Parse(record.GetValue().ToString()));
                    }
                       )).ToList();
            });
            return Result.Ok(result);
        }

        public async Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByProperty(Guid propertyId, TotalTimePeriod totalTimePeriod, Guid value, bool isConsumed)
        {
            var admin = await _userRepository.FindById(value);
            if (admin == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            if (admin.Role != Role.ADMIN && admin.Role != Role.SUPERADMIN)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            var property = await _smartPropertyRepository.FindById(propertyId);
            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));

            }
            var timePeriod = GetTimePeriod(totalTimePeriod);
            var getMeanFromTimePeriod = GetTimePeriod(totalTimePeriod);
            var flux = "";
            if (isConsumed)
            {
                flux =
                  "from(bucket: \"smarthome\")\n" +
                  "  |> range(start: -" + timePeriod + "h)\n" +
                  "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
                  "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + property.Id.ToString() + "\")\n" +
                  "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: sum, createEmpty: false)\n";
            }
            else
            {
                flux =
                 "from(bucket: \"smarthome\")\n" +
                 "  |> range(start: -" + timePeriod + "h)\n" +
                 "  |> filter(fn: (r) => r[\"_measurement\"] == \"electricenergy\")\n" +
                 "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + property.Id.ToString() + "\")\n" +
                 "  |> filter(fn: (r) => r[\"_field\"] == \"totalProduced\")\n" +
                 "  |> aggregateWindow(every: " + getMeanFromTimePeriod + "m, fn: sum, createEmpty: false)\n";
            }
            var result = await _influxService.QueryAsync(async query =>
            {

                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                    {
                        return new EnergyConsumptionDTO(DateTime.Parse(record.GetTime().ToString()), double.Parse(record.GetValue().ToString()));
                    }
                       )).ToList();
            });
            return Result.Ok(result);
        }

        public async Task<Result<List<EnergyConsumptionDTO>>> GetEnergyConsumedProducedByPropertyInRange(Guid propertyId, string from, string to, Guid value, bool isConsumed)
        {
            var admin = await _userRepository.FindById(value);
            if (admin == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            if (admin.Role != Role.ADMIN && admin.Role != Role.SUPERADMIN)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFoundUserId());
            }
            var property = await _smartPropertyRepository.FindById(propertyId);
            if (property == null)
            {
                return ResultExtensions.FailNotFound(ErrorMessages.NotFound("property", "id"));

            }
            if (!DateTime.TryParse(from, out _) || !DateTime.TryParse(to, out _))
            {
                return ResultExtensions.FailBadRequest("Invalid date format for 'from' or 'to' parameters.");
            }
            DateTime parsedStartDate = DateTime.Parse(from);
            DateTime parsedEndDate = DateTime.Parse(to);
            if (parsedStartDate >= parsedEndDate)
            {
                return ResultExtensions.FailBadRequest("Start Date must be before end date.");

            }
            if (parsedStartDate >= DateTime.Now)
            {
                return ResultExtensions.FailBadRequest("Start Date must be before current date.");

            }
            TimeSpan difference = parsedEndDate - parsedStartDate;
            bool isMonthApart = difference.TotalDays >= 31;
            if (isMonthApart)
            {
                return ResultExtensions.FailBadRequest("The difference between dates is more than a month.");

            }
            var formattedStartDate = parsedStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var formattedEndDate = parsedEndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var flux = "";
            if (isConsumed)
            {
                flux =
              "from(bucket: \"smarthome\")\n" +
              "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
              "  |> filter(fn: (r) => r[\"_measurement\"] == \"energyconsumed\")\n" +
              "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + property.Id.ToString() + "\")\n" +
              "  |> aggregateWindow(every: 30m, fn: sum, createEmpty: false)\n";
            }
            else
            {
                flux =
             "from(bucket: \"smarthome\")\n" +
             "  |> range(start: " + formattedStartDate + ", stop: " + formattedEndDate + ")\n" +
             "  |> filter(fn: (r) => r[\"_measurement\"] == \"electricenergy\")\n" +
             "  |> filter(fn: (r) => r[\"smartPropertyId\"] == \"" + property.Id.ToString() + "\")\n" +
             "  |> filter(fn: (r) => r[\"_field\"] == \"totalProduced\")\n" +
             "  |> aggregateWindow(every: 20m, fn: sum, createEmpty: false)\n";
            }
            var result = await _influxService.QueryAsync(async query =>
            {

                var tables = await query.QueryAsync(flux, "FTN");
                return tables.SelectMany(table =>
                    table.Records.Select(record =>
                    {
                        return new EnergyConsumptionDTO(DateTime.Parse(record.GetTime().ToString()), double.Parse(record.GetValue().ToString()));
                    }
                       )).ToList();
            });
            return Result.Ok(result);
        }
    }
}
