using SmartHouse.Core.Model;
using SmartHouse.Core.Model.OutsideSmartDevices;
using SmartHouse.Core.Model.SmartHomeDevices;
using SmartHouse.Infrastructure;
using System.Security.Cryptography;

namespace SmartHouse
{
    public class Seed
    {
        private readonly DataContext _dataContext;
        public Seed(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        private static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            char[] randomChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomChars);
        }

        static void SaveToSuperAdminFile(string fileName, string email, string password, string username)
        {

            File.WriteAllText(fileName, $"email: {email}\npassword: {password}\nusername: {username}");
        }
        static void SaveUsersToFile(string fileName, string email, string password, Role role)
        {

            File.AppendAllText(fileName, $"email: {email}\npassword: {password}\nrole: {role.ToString()}\n");
        }
        public void SeedDataContext()
        {

            if (!_dataContext.Users.Any())
            {
                SeedUsers();
                _dataContext.SaveChanges();


            }
            if (!_dataContext.Cycles.Any())
            {
                SeedCycles();
                _dataContext.SaveChanges();

            }

            if (!_dataContext.Countries.Any())
            {
                SeedCountries();
                _dataContext.SaveChanges();

            }

            if (!_dataContext.SmartProperties.Any())
            {
                SeedSmartProperties();
                _dataContext.SaveChanges();

            }
        }

        private void SeedSmartProperties()
        {
            var random = new Random();
            for (int i = 0; i < 12; i++)
            {
                var quadrature = random.Next(1, 300);
                var numOfFloors = random.Next(1, 4);
                var userEmail = "";
                if (i % 2 == 0)
                {
                    userEmail = "nebojsavuga01@gmail.com";
                }
                else if (i % 3 == 0)
                {
                    userEmail = "bogdanjanosevic@gmail.com";

                }
                else
                {
                    userEmail = "dusanbibin2@gmail.com";

                }
                var user = _dataContext.Users.Where(user => user.Email == userEmail).FirstOrDefault();

                var property = new SmartProperty()
                {
                    TypeOfProperty = TypeOfProperty.House,
                    Name = GenerateRandomString(6),
                    Address = "Polgar Andrasa 34",
                    City = "Novi Sad",
                    Country = "Serbia",
                    Quadrature = quadrature,
                    IsAccepted = Activation.Accepted,
                    NumberOfFloors = numOfFloors,
                    Latitude = 45.246342,
                    Longitude = 19.817730,
                    PathToImage = "images/smarthouse.jpg",
                    Reason = "",
                    Devices = new List<SmartDevice>
                {
                    new Lamp()
                    {
                        Name = GenerateRandomString(6),
                        PathToImage = "images/smarthouse.jpg",
                        IsOnline = false,
                        IsOn = false,
                        TypeOfPowerSupply = TypeOfPowerSupply.Battery,
                        PowerUsage = 0,
                        LastPingTime = DateTime.MinValue,
                        Luminosity = 20,
                    },

                    new AmbientSensor()
                    {
                        Name = GenerateRandomString(6),
                        PathToImage = "images/smarthouse.jpg",
                        IsOnline = false,
                        IsOn = false,
                        TypeOfPowerSupply = TypeOfPowerSupply.Battery,
                        PowerUsage = 0,
                        LastPingTime = DateTime.MinValue,
                        RoomTemperature = 22,
                        RoomHumidity = 40,
                    },

                    new AirConditioner()
                    {
                        Name = GenerateRandomString(6),
                        PathToImage = "images/smarthouse.jpg",
                        IsOnline = false,
                        IsOn = false,
                        TypeOfPowerSupply = TypeOfPowerSupply.Battery,
                        PowerUsage = 0,
                        LastPingTime = DateTime.MinValue,
                        MinTemperature = 14,
                        MaxTemperature = 26,
                        CurrentMode = Mode.Automatic,
                        Modes = new List<Mode> { Mode.Automatic, Mode.Heating, Mode.Ventilation, Mode.Cooling }
                    },

                    new WashingMachine()
                    {
                        Name = GenerateRandomString(6),
                        PathToImage = "images/smarthouse.jpg",
                        IsOnline = false,
                        IsOn = false,
                        TypeOfPowerSupply = TypeOfPowerSupply.Battery,
                        PowerUsage = 0,
                        LastPingTime = DateTime.MinValue,
                        SupportedCycles = _dataContext.Cycles.ToList()
                    },
                    new VehicleGate()
                    {
                        Name = GenerateRandomString(6),
                        PathToImage = "images/smarthouse.jpg",
                        IsOnline = false,
                        IsOn = false,
                        TypeOfPowerSupply = TypeOfPowerSupply.Battery,
                        PowerUsage = 0,
                        LastPingTime = DateTime.MinValue,
                        IsPublic = false,
                        ValidLicensePlates = new List<string> { "BG 123-AB", "NS 456 - CD", "KG 789-EF", "SI 022-CF" }
                    },
                    new SprinklerSystem()
                    {
                        Name = GenerateRandomString(6),
                        PathToImage = "images/smarthouse.jpg",
                        IsOnline = false,
                        IsOn = false,
                        TypeOfPowerSupply = TypeOfPowerSupply.Battery,
                        PowerUsage = 0,
                        LastPingTime = DateTime.MinValue,
                        IsSpecialMode  = false,
                        StartSprinkle = new TimeSpan(0, 0, 0),
                        EndSprinkle = new TimeSpan(0, 0, 0),
                        ActiveDays = new List<string> {}
                    }
                }

                };
                _dataContext.SmartProperties.Add(property);
                user.Properties.Add(property);
            }

        }

        private void SeedUsers()
        {
            SeedSuperAdmin();
            File.WriteAllText("confidental/users.txt", "");

            SeedUserByType(Role.ADMIN);
            SeedUserByType(Role.USER);
            SeedOurUsers();
        }

        private void SeedOurUsers()
        {
            string password = "Nebojsa123";
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(password, salt);
            var nebojsa = new User
            {
                Role = Role.USER,
                Name = "Nebojsa",
                LastName = "Vuga",
                Email = "nebojsavuga01@gmail.com",
                UserName = "nebojsavuga",
                Salt = Convert.ToBase64String(salt),
                ProfilePicturePath = "images/users/user.jpg",
                Password = Convert.ToBase64String(hashedPassword),
                IsVerified = true,

            };
            _dataContext.Users.Add(nebojsa);
            SaveUsersToFile("confidental/users.txt", nebojsa.Email, password, nebojsa.Role);
            password = "Bogdan123";
            salt = GenerateSalt();
            hashedPassword = HashPassword(password, salt);
            var bogdan = new User
            {
                Role = Role.USER,
                Name = "Bogdan",
                LastName = "Janosevic",
                Email = "bogdanjanosevic@gmail.com",
                UserName = "bogdanjanosevic",
                Salt = Convert.ToBase64String(salt),
                ProfilePicturePath = "images/users/user.jpg",
                Password = Convert.ToBase64String(hashedPassword),
                IsVerified = true,

            };
            _dataContext.Users.Add(bogdan);
            SaveUsersToFile("confidental/users.txt", bogdan.Email, password, bogdan.Role);
            password = "Dusan123";
            salt = GenerateSalt();
            hashedPassword = HashPassword(password, salt);
            var bibin = new User
            {
                Role = Role.USER,
                Name = "Dusan",
                LastName = "Bibin",
                Email = "dusanbibin2@gmail.com",
                UserName = "dusanbibin",
                Salt = Convert.ToBase64String(salt),
                ProfilePicturePath = "images/users/user.jpg",
                Password = Convert.ToBase64String(hashedPassword),
                IsVerified = true,

            };
            _dataContext.Users.Add(bibin);
            SaveUsersToFile("confidental/users.txt", bibin.Email, password, bibin.Role);

        }

        private void SeedUserByType(Role role)
        {
            for (int i = 0; i < 1000; i++)
            {
                string email = GenerateRandomString(8 + i % 10) + "@gmail.com";
                string password = "Sifra123";
                byte[] salt = GenerateSalt();
                byte[] hashedPassword = HashPassword(password, salt);
                _dataContext.Users.Add(new User
                {
                    Role = role,
                    Name = GenerateRandomString(6 + i % 10),
                    LastName = GenerateRandomString(6 + i % 10),
                    Email = email,
                    UserName = GenerateRandomString(6 + i % 10),
                    Salt = Convert.ToBase64String(salt),
                    ProfilePicturePath = "images/users/user.jpg",
                    Password = Convert.ToBase64String(hashedPassword),
                    IsVerified = true,

                });
                SaveUsersToFile("confidental/users.txt", email, password, role);
            }
        }

        private void SeedSuperAdmin()
        {
            string email = GenerateRandomString(12) + "@gmail.com";
            string password = GenerateRandomString(32);
            byte[] salt = GenerateSalt();
            byte[] hashedPassword = HashPassword(password, salt);
            _dataContext.Users.Add(new User
            {
                Role = Role.SUPERADMIN,
                Name = "super",
                LastName = "admin",
                Email = email,
                UserName = "admin",
                Salt = Convert.ToBase64String(salt),
                ProfilePicturePath = "images/avatar.png",
                Password = Convert.ToBase64String(hashedPassword),
                IsVerified = true,
                IsPasswordChanged = false
            });
            SaveToSuperAdminFile("confidental/superadmin.txt", email, password, "admin");
        }
        private void SeedCountries()
        {
            Country serbia = new Country
            {
                Name = "Serbia",
                Cities = new List<City>
            {
                new City
                {
                    Name = "Novi Sad",
                    Latitude = 45.2671,
                    Longitude = 19.8335
                },
                new City
                {
                    Name = "Belgrade",
                    Latitude = 44.7866,
                    Longitude = 20.4489
                },
                new City
                {
                    Name = "Nis",
                    Latitude = 43.3194,
                    Longitude = 21.8963
                },
                new City
                {
                    Name = "Subotica",
                    Latitude = 46.1014,
                    Longitude = 19.6658
                },
                new City
                {
                    Name = "Kragujevac",
                    Latitude = 44.0128,
                    Longitude = 20.9112
                },
                new City
                {
                    Name = "Cacak",
                    Latitude = 43.8927,
                    Longitude = 20.3443
                },
                new City
                {
                    Name = "Sremska Mitrovica",
                    Latitude = 44.9833,
                    Longitude = 19.6167
                },
                new City
                {
                    Name = "Sid",
                    Latitude = 45.126473,
                    Longitude =  19.221493
                },
                new City
                {
                    Name = "Zrenjanin",
                    Latitude = 45.381561,
                    Longitude = 20.368574
                },
                new City
                {
                    Name = "Lacarak",
                    Latitude = 45.000340,
                    Longitude = 19.564601
                }
            }
            };
            Country france = new Country
            {
                Name = "France",
                Cities = new List<City>
                    {
                        new City { Name = "Paris", Latitude = 48.8566, Longitude = 2.3522 },
                        new City { Name = "Marseille", Latitude = 43.2965, Longitude = 5.3698 },
                        new City { Name = "Lyon", Latitude = 45.7640, Longitude = 4.8357 },
                        new City { Name = "Toulouse", Latitude = 43.6047, Longitude = 1.4442 },
                        new City { Name = "Nice", Latitude = 43.7102, Longitude = 7.2620 },
                        new City { Name = "Nantes", Latitude = 47.2184, Longitude = -1.5536 },
                        new City { Name = "Strasbourg", Latitude = 48.8566, Longitude = 2.3522 },
                        new City { Name = "Montpellier", Latitude = 43.6110, Longitude = 3.8767 },
                        new City { Name = "Bordeaux", Latitude = 44.8378, Longitude = -0.5792 },
                        new City { Name = "Lille", Latitude = 50.6292, Longitude = 3.0573 }
                    }
            };
            Country canada = new Country
            {
                Name = "Canada",
                Cities = new List<City>
                {
                    new City { Name = "Toronto", Latitude = 43.6532, Longitude = -79.3832 },
                    new City { Name = "Montreal", Latitude = 45.5017, Longitude = -73.5673 },
                    new City { Name = "Vancouver", Latitude = 49.2827, Longitude = -123.1207 },
                    new City { Name = "Calgary", Latitude = 51.0486, Longitude = -114.0708 },
                    new City { Name = "Edmonton", Latitude = 53.5444, Longitude = -113.4909 },
                    new City { Name = "Ottawa", Latitude = 45.4215, Longitude = -75.6993 },
                    new City { Name = "Quebec City", Latitude = 46.8139, Longitude = -71.2080 },
                    new City { Name = "Winnipeg", Latitude = 49.8951, Longitude = -97.1384 },
                    new City { Name = "Hamilton", Latitude = 43.2557, Longitude = -79.8711 },
                    new City { Name = "Halifax", Latitude = 44.6488, Longitude = -63.5752 }
                }
            };
            Country brazil = new Country
            {
                Name = "Brazil",
                Cities = new List<City>
                {
                    new City { Name = "Sao Paulo", Latitude = -23.5505, Longitude = -46.6333 },
                    new City { Name = "Rio de Janeiro", Latitude = -22.9068, Longitude = -43.1729 },
                    new City { Name = "Brasilia", Latitude = -15.8267, Longitude = -47.9218 },
                    new City { Name = "Salvador", Latitude = -12.9714, Longitude = -38.5014 },
                    new City { Name = "Fortaleza", Latitude = -3.7172, Longitude = -38.5433 },
                    new City { Name = "Belo Horizonte", Latitude = -19.9167, Longitude = -43.9345 },
                    new City { Name = "Manaus", Latitude = -3.1190, Longitude = -60.0217 },
                    new City { Name = "Curitiba", Latitude = -25.4296, Longitude = -49.2719 },
                    new City { Name = "Recife", Latitude = -8.0476, Longitude = -34.8770 },
                    new City { Name = "Porto Alegre", Latitude = -30.0330, Longitude = -51.2200 }
                }
            };
            Country england = new Country
            {
                Name = "England",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "London",
                        Latitude = 51.5099,
                        Longitude = -0.1180
                    },
                    new City
                    {
                        Name = "Manchester",
                        Latitude = 53.4830,
                        Longitude = -2.2441
                    },
                    new City
                    {
                        Name = "Birmingham",
                        Latitude = 52.4862,
                        Longitude = -1.8904
                    },
                    new City
                    {
                        Name = "Liverpool",
                        Latitude = 53.4084,
                        Longitude = -2.9916
                    },
                    new City
                    {
                        Name = "Leeds",
                        Latitude = 53.8008,
                        Longitude = -1.5491
                    }
                }
            };
            Country greece = new Country
            {
                Name = "Greece",
                Cities = new List<City>
            {
                new City
                {
                    Name = "Athens",
                    Latitude = 37.9838,
                    Longitude = 23.7275
                },
                new City
                {
                    Name = "Thessaloniki",
                    Latitude = 40.6401,
                    Longitude = 22.9444
                },
                new City
                {
                    Name = "Patras",
                    Latitude = 38.2545,
                    Longitude = 21.7346
                },
                new City
                {
                    Name = "Heraklion",
                    Latitude = 35.3387,
                    Longitude = 25.1442
                },
                new City
                {
                    Name = "Larissa",
                    Latitude = 39.6357,
                    Longitude = 22.4173
                }
            }

            };
            Country russia = new Country
            {
                Name = "Russia",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "Moscow",
                        Latitude = 55.7558,
                        Longitude = 37.6176
                    },
                    new City
                    {
                        Name = "Saint Petersburg",
                        Latitude = 59.9343,
                        Longitude = 30.3351
                    },
                    new City
                    {
                        Name = "Novosibirsk",
                        Latitude = 55.0084,
                        Longitude = 82.9357
                    },
                    new City
                    {
                        Name = "Yekaterinburg",
                        Latitude = 56.8389,
                        Longitude = 60.6057
                    },
                    new City
                    {
                        Name = "Kazan",
                        Latitude = 55.7961,
                        Longitude = 49.1067
                    }
                }
            };
            Country spain = new Country
            {
                Name = "Spain",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "Madrid",
                        Latitude = 40.4168,
                        Longitude = -3.7038
                    },
                    new City
                    {
                        Name = "Barcelona",
                        Latitude = 41.3851,
                        Longitude = 2.1734
                    },
                    new City
                    {
                        Name = "Valencia",
                        Latitude = 39.4699,
                        Longitude = -0.3763
                    },
                    new City
                    {
                        Name = "Seville",
                        Latitude = 37.3886,
                        Longitude = -5.9822
                    },
                    new City
                    {
                        Name = "Zaragoza",
                        Latitude = 41.6488,
                        Longitude = -0.8891
                    }
                }
            };
            Country egypt = new Country
            {
                Name = "Egypt",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "Cairo",
                        Latitude = 30.033,
                        Longitude = 31.233
                    },
                    new City
                    {
                        Name = "Alexandria",
                        Latitude = 31.2156,
                        Longitude = 29.9553
                    },
                    new City
                    {
                        Name = "Luxor",
                        Latitude = 25.6872,
                        Longitude = 32.6396
                    },
                    new City
                    {
                        Name = "Aswan",
                        Latitude = 24.0889,
                        Longitude = 32.8998
                    },
                    new City
                    {
                        Name = "Sharm El Sheikh",
                        Latitude = 27.9158,
                        Longitude = 34.3297
                    }
                }
            };
            Country australia = new Country
            {
                Name = "Australia",
                Cities = new List<City>
    {
                new City
                {
                    Name = "Sydney",
                    Latitude = -33.8688,
                    Longitude = 151.2093
                },
                new City
                {
                    Name = "Melbourne",
                    Latitude = -37.8136,
                    Longitude = 144.9631
                },
                new City
                {
                    Name = "Brisbane",
                    Latitude = -27.4698,
                    Longitude = 153.0251
                },
                new City
                {
                    Name = "Perth",
                    Latitude = -31.9505,
                    Longitude = 115.8605
                },
                new City
                {
                    Name = "Adelaide",
                    Latitude = -34.9285,
                    Longitude = 138.6007
                }
            }
            };
            Country usa = new Country
            {
                Name = "United States",
                Cities = new List<City>
    {
                    new City
                    {
                        Name = "New York",
                        Latitude = 40.7128,
                        Longitude = -74.0060
                    },
                    new City
                    {
                        Name = "Los Angeles",
                        Latitude = 34.0522,
                        Longitude = -118.2437
                    },
                    new City
                    {
                        Name = "Chicago",
                        Latitude = 41.8781,
                        Longitude = -87.6298
                    },
                    new City
                    {
                        Name = "Houston",
                        Latitude = 29.7604,
                        Longitude = -95.3698
                    },
                    new City
                    {
                        Name = "Miami",
                        Latitude = 25.7617,
                        Longitude = -80.1918
                    }
                }
            };
            Country germany = new Country
            {
                Name = "Germany",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "Berlin",
                        Latitude = 52.5200,
                        Longitude = 13.4050
                    },
                    new City
                    {
                        Name = "Hamburg",
                        Latitude = 53.5511,
                        Longitude = 9.9937
                    },
                    new City
                    {
                        Name = "Munich",
                        Latitude = 48.8566,
                        Longitude = 2.3522
                    },
                    new City
                    {
                        Name = "Cologne",
                        Latitude = 50.9375,
                        Longitude = 6.9603
                    },
                    new City
                    {
                        Name = "Frankfurt",
                        Latitude = 50.1109,
                        Longitude = 8.6821
                    },
                    new City
                    {
                        Name = "Stuttgart",
                        Latitude = 48.7758,
                        Longitude = 9.1829
                    },
                    new City
                    {
                        Name = "Dusseldorf",
                        Latitude = 51.2277,
                        Longitude = 6.7735
                    },
                    new City
                    {
                        Name = "Dortmund",
                        Latitude = 51.5136,
                        Longitude = 7.4653
                    },
                    new City
                    {
                        Name = "Essen",
                        Latitude = 51.4556,
                        Longitude = 7.0116
                    },
                    new City
                    {
                        Name = "Leipzig",
                        Latitude = 51.3397,
                        Longitude = 12.3731
                    }
                }
            };
            Country slovenia = new Country
            {
                Name = "Slovenia",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "Ljubljana",
                        Latitude = 46.0569,
                        Longitude = 14.5058
                    },
                    new City
                    {
                        Name = "Maribor",
                        Latitude = 46.5547,
                        Longitude = 15.6466
                    },
                    new City
                    {
                        Name = "Celje",
                        Latitude = 46.2389,
                        Longitude = 15.2675
                    },
                    new City
                    {
                        Name = "Kranj",
                        Latitude = 46.2439,
                        Longitude = 14.3559
                    },
                    new City
                    {
                        Name = "Novo Mesto",
                        Latitude = 45.8012,
                        Longitude = 15.1717
                    },
                    new City
                    {
                        Name = "Velenje",
                        Latitude = 46.3592,
                        Longitude = 15.1107
                    },
                    new City
                    {
                        Name = "Ptuj",
                        Latitude = 46.4196,
                        Longitude = 15.8700
                    },
                    new City
                    {
                        Name = "Trbovlje",
                        Latitude = 46.1556,
                        Longitude = 15.0533
                    },
                    new City
                    {
                        Name = "Koper",
                        Latitude = 45.5469,
                        Longitude = 13.7294
                    },
                    new City
                    {
                        Name = "Domzale",
                        Latitude = 46.1380,
                        Longitude = 14.5975
                    }
                }
            };
            Country poland = new Country
            {
                Name = "Poland",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "Warsaw",
                        Latitude = 52.2297,
                        Longitude = 21.0122
                    },
                    new City
                    {
                        Name = "Krakow",
                        Latitude = 50.0647,
                        Longitude = 19.9450
                    },
                    new City
                    {
                        Name = "Wroclaw",
                        Latitude = 51.1079,
                        Longitude = 17.0385
                    },
                    new City
                    {
                        Name = "Poznan",
                        Latitude = 52.4064,
                        Longitude = 16.9252
                    },
                    new City
                    {
                        Name = "Gdansk",
                        Latitude = 54.3520,
                        Longitude = 18.6466
                    },
                    new City
                    {
                        Name = "Szczecin",
                        Latitude = 53.4285,
                        Longitude = 14.5528
                    },
                    new City
                    {
                        Name = "Katowice",
                        Latitude = 50.2649,
                        Longitude = 19.0238
                    },
                    new City
                    {
                        Name = "Lodz",
                        Latitude = 51.7592,
                        Longitude = 19.4558
                    },
                    new City
                    {
                        Name = "Bydgoszcz",
                        Latitude = 53.1235,
                        Longitude = 18.0084
                    },
                    new City
                    {
                        Name = "Bialystok",
                        Latitude = 53.1325,
                        Longitude = 23.1688
                    }
                }
            };
            Country sweden = new Country
            {
                Name = "Sweden",
                Cities = new List<City>
                {
                    new City
                    {
                        Name = "Stockholm",
                        Latitude = 59.3293,
                        Longitude = 18.0686
                    },
                    new City
                    {
                        Name = "Gothenburg",
                        Latitude = 57.7089,
                        Longitude = 11.9746
                    },
                    new City
                    {
                        Name = "Malmo",
                        Latitude = 55.6049,
                        Longitude = 13.0038
                    },
                    new City
                    {
                        Name = "Uppsala",
                        Latitude = 59.8586,
                        Longitude = 17.6389
                    },
                    new City
                    {
                        Name = "Linkoping",
                        Latitude = 58.4108,
                        Longitude = 15.6214
                    },
                    new City
                    {
                        Name = "Vasteras",
                        Latitude = 59.6093,
                        Longitude = 16.5448
                    },
                    new City
                    {
                        Name = "Orebro",
                        Latitude = 59.2753,
                        Longitude = 15.2134
                    },
                    new City
                    {
                        Name = "Helsingborg",
                        Latitude = 56.0465,
                        Longitude = 12.6944
                    },
                    new City
                    {
                        Name = "Norrkoping",
                        Latitude = 58.5877,
                        Longitude = 16.1920
                    },
                    new City
                    {
                        Name = "Jonkoping",
                        Latitude = 57.7826,
                        Longitude = 14.1616
                    }
                }
            };

            Country italy = new Country
            {
                Name = "Italy",
                Cities = new List<City>
            {
                new City
                {
                    Name = "Rome",
                    Latitude = 41.9028,
                    Longitude = 12.4964
                },
                new City
                {
                    Name = "Milan",
                    Latitude = 45.4642,
                    Longitude = 9.1900
                },
                new City
                {
                    Name = "Naples",
                    Latitude = 40.8522,
                    Longitude = 14.2681
                },
                new City
                {
                    Name = "Florence",
                    Latitude = 43.7696,
                    Longitude = 11.2558
                },
                new City
                {
                    Name = "Venice",
                    Latitude = 45.4408,
                    Longitude = 12.3155
                }
            }
            };


            _dataContext.Countries.AddRange(serbia, greece, italy, england, russia, spain, germany, poland, sweden, slovenia, australia, egypt, usa, france, canada, brazil);
        }

        private void SeedCycles()
        {
            var cycles = new List<Cycle>()
            {
                new (){Name=CycleName.Cotton, Temperature=30, Duration=60},
                new (){Name=CycleName.Wool, Temperature=30, Duration=60},
                new (){Name=CycleName.Synthetics, Temperature=30, Duration=60},
                new (){Name=CycleName.Cotton, Temperature=60, Duration=120},
                new (){Name=CycleName.Wool, Temperature=60, Duration=120},
                new (){Name=CycleName.Synthetics, Temperature = 60, Duration = 120},
                new (){Name=CycleName.Cotton, Temperature=90, Duration=180},
                new (){Name=CycleName.Synthetics, Temperature=90, Duration=180},
                new (){Name=CycleName.Delicate, Temperature=30, Duration=60},
                new (){Name=CycleName.QuickWash, Temperature=40, Duration=30},
                new (){Name=CycleName.EcoWash, Temperature=40, Duration=60},
            };
            _dataContext.Cycles.AddRange(cycles);
        }

        static byte[] HashPassword(string password, byte[] salt)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32);
            }
        }

        static byte[] GenerateSalt()
        {
            byte[] salt = new byte[32];
            using (RNGCryptoServiceProvider rng = new())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }

}