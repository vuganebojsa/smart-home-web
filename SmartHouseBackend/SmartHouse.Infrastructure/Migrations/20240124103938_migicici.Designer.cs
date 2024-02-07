﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartHouse.Infrastructure;

#nullable disable

namespace SmartHouse.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240124103938_migicici")]
    partial class migicici
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CycleWashingMachine", b =>
                {
                    b.Property<Guid>("SupportedCyclesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("WashingMachineId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("SupportedCyclesId", "WashingMachineId");

                    b.HasIndex("WashingMachineId");

                    b.ToTable("CycleWashingMachine");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.Cycle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Duration")
                        .HasColumnType("float");

                    b.Property<int>("Name")
                        .HasColumnType("int");

                    b.Property<int>("Temperature")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Cycles");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.Panel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Efficency")
                        .HasColumnType("float");

                    b.Property<double>("Size")
                        .HasColumnType("float");

                    b.Property<Guid>("SolarPanelSystemId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SolarPanelSystemId");

                    b.ToTable("Panels");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.Permission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DeviceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("GrantDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsValid")
                        .HasColumnType("bit");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RecipientId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("OwnerId");

                    b.HasIndex("RecipientId");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartDevice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsOn")
                        .HasColumnType("bit");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastPingTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PathToImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("PowerUsage")
                        .HasColumnType("float");

                    b.Property<Guid?>("SmartPropertyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("TypeOfPowerSupply")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SmartPropertyId");

                    b.ToTable("SmartDevices");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SmartDevice");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartProperty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("IsAccepted")
                        .HasColumnType("int");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumberOfFloors")
                        .HasColumnType("int");

                    b.Property<string>("PathToImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Quadrature")
                        .HasColumnType("float");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TypeOfProperty")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SmartProperties");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.TypeOfDevice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("TypeOfDevices");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsPasswordChanged")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProfilePicturePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.ElectromagneticDevices.ElectricVehicleCharger", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.Property<int>("NumberOfConnections")
                        .HasColumnType("int");

                    b.Property<double>("PercentageOfCharge")
                        .HasColumnType("float");

                    b.Property<double>("Power")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("ElectricVehicleCharger");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.ElectromagneticDevices.HouseBattery", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.Property<double>("BatterySize")
                        .HasColumnType("float");

                    b.Property<double>("OccupationLevel")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("HouseBattery");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.ElectromagneticDevices.SolarPanelSystem", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.HasDiscriminator().HasValue("SolarPanelSystem");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.OutsideSmartDevices.Lamp", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.Property<double>("Luminosity")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("Lamp");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.OutsideSmartDevices.SprinklerSystem", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.HasDiscriminator().HasValue("SprinklerSystem");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.OutsideSmartDevices.VehicleGate", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("ValidLicensePlates")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("VehicleGate");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartHomeDevices.AirConditioner", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.Property<int>("CurrentMode")
                        .HasColumnType("int");

                    b.Property<double>("MaxTemperature")
                        .HasColumnType("float");

                    b.Property<double>("MinTemperature")
                        .HasColumnType("float");

                    b.Property<string>("Modes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("AirConditioner");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartHomeDevices.AmbientSensor", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.Property<double>("RoomHumidity")
                        .HasColumnType("float");

                    b.Property<double>("RoomTemperature")
                        .HasColumnType("float");

                    b.HasDiscriminator().HasValue("AmbientSensor");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartHomeDevices.WashingMachine", b =>
                {
                    b.HasBaseType("SmartHouse.Core.Model.SmartDevice");

                    b.Property<Guid?>("CurrentCycleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("CurrentCycleId");

                    b.HasDiscriminator().HasValue("WashingMachine");
                });

            modelBuilder.Entity("CycleWashingMachine", b =>
                {
                    b.HasOne("SmartHouse.Core.Model.Cycle", null)
                        .WithMany()
                        .HasForeignKey("SupportedCyclesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartHouse.Core.Model.SmartHomeDevices.WashingMachine", null)
                        .WithMany()
                        .HasForeignKey("WashingMachineId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SmartHouse.Core.Model.City", b =>
                {
                    b.HasOne("SmartHouse.Core.Model.Country", "Country")
                        .WithMany("Cities")
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.Panel", b =>
                {
                    b.HasOne("SmartHouse.Core.Model.ElectromagneticDevices.SolarPanelSystem", "SolarPanelSystem")
                        .WithMany("Panels")
                        .HasForeignKey("SolarPanelSystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SolarPanelSystem");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.Permission", b =>
                {
                    b.HasOne("SmartHouse.Core.Model.SmartDevice", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("SmartHouse.Core.Model.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartHouse.Core.Model.User", "Recipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("Owner");

                    b.Navigation("Recipient");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartDevice", b =>
                {
                    b.HasOne("SmartHouse.Core.Model.SmartProperty", "SmartProperty")
                        .WithMany("Devices")
                        .HasForeignKey("SmartPropertyId");

                    b.Navigation("SmartProperty");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartProperty", b =>
                {
                    b.HasOne("SmartHouse.Core.Model.User", "User")
                        .WithMany("Properties")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartHomeDevices.WashingMachine", b =>
                {
                    b.HasOne("SmartHouse.Core.Model.Cycle", "CurrentCycle")
                        .WithMany()
                        .HasForeignKey("CurrentCycleId");

                    b.Navigation("CurrentCycle");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.Country", b =>
                {
                    b.Navigation("Cities");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.SmartProperty", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.User", b =>
                {
                    b.Navigation("Properties");
                });

            modelBuilder.Entity("SmartHouse.Core.Model.ElectromagneticDevices.SolarPanelSystem", b =>
                {
                    b.Navigation("Panels");
                });
#pragma warning restore 612, 618
        }
    }
}
