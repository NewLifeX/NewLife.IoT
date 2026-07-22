using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NewLife;
using NewLife.IoT;
using NewLife.IoT.Drivers;
using NewLife.IoT.Models;
using NewLife.IoT.ThingModels;
using NewLife.IoT.ThingSpecification;
using NewLife.IoTDatabase.Drivers;
using XCode.DataAccessLayer;
using Xunit;

namespace XUnitTest;

/// <summary>数据库驱动真实 SQLite 测试。覆盖 DB-1</summary>
public class DatabaseDriverTests
{
    #region 辅助

    private class MiniDevice : IDevice
    {
        public String Code { get; set; } = "db-device";
        public IDictionary<String, Object?> Properties { get; } = new Dictionary<String, Object?>();
        public ThingSpec? Specification { get; set; }
        public IPoint[]? Points { get; set; }
        public IDictionary<String, Delegate> Services { get; } = new Dictionary<String, Delegate>();

        public Task StartAsync(CancellationToken ct) => Task.CompletedTask;
        public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
        public Task<IDeviceInfo[]> SetOnlineAsync(IDeviceInfo[] d, CancellationToken ct) => Task.FromResult(d);
        public Task<IDeviceInfo[]> SetOfflineAsync(String[] d, CancellationToken ct) => Task.FromResult(Array.Empty<IDeviceInfo>());
        public Task<Int32> PostPropertyAsync(String code, Object items, CancellationToken ct) => Task.FromResult(0);
        public Task<Int32> PostDataAsync(String code, DataModel[] items, CancellationToken ct) => Task.FromResult(0);
        public Task<Int32> PostEventAsync(String code, EventModel[] items, CancellationToken ct) => Task.FromResult(0);
        public void PostProperty() { }
        public void SetProperty(String name, Object? value) { }
        public Boolean AddData(String name, String value) => true;
        public Boolean WriteEvent(String type, String name, String remark) => true;
        public void RegisterService(String service, Delegate method) { }
    }

    #endregion

    // ===== DB-1 IoTDatabaseDriver =====

    [Fact]
    public void IoTDatabaseDriver_Attribute()
    {
        var attr = typeof(IoTDatabaseDriver).GetCustomAttributes(typeof(DriverAttribute), false);
        Assert.Single(attr);
        Assert.Equal("IoTDatabase", (attr[0] as DriverAttribute)!.Name);
    }

    [Fact]
    public void IoTDatabaseDriver_GetSpecification()
    {
        var driver = new IoTDatabaseDriver();
        // IoTDatabaseDriver 未重写 OnGetSpecification，基类返回 null
        var spec = driver.GetSpecification();
        Assert.Null(spec);
    }

    [Fact]
    public async Task IoTDatabaseDriver_OpenAsync_WithSqlite()
    {
        // 创建临时 SQLite 数据库
        var dbFile = Path.GetTempFileName();
        try
        {
            var connStr = $"Data Source={dbFile}";
            var connName = connStr.GetBytes().Crc().GetBytes().ToHex();
            if (!DAL.ConnStrs.ContainsKey(connName))
                DAL.AddConnStr(connName, connStr, null, "SQLite");
            var dal = DAL.Create(connName);

            // 建表插入测试数据
            dal.Execute("CREATE TABLE IF NOT EXISTS IoT_Test (Id INTEGER PRIMARY KEY, Name TEXT, Value TEXT)");
            dal.Execute("INSERT INTO IoT_Test (Name, Value) VALUES ('Temp', '25.5')");
            dal.Execute("INSERT INTO IoT_Test (Name, Value) VALUES ('Humidity', '60')");

            var driver = new IoTDatabaseDriver();
            var param = new DatabaseParameter
            {
                ConnectionString = connStr,
                DatabaseType = DatabaseType.SQLite,
                QuerySql = "SELECT * FROM IoT_Test",
                CaptureAll = false,
            };
            var device = new MiniDevice();
            var node = await driver.OpenAsync(device, param);

            Assert.NotNull(node);
            Assert.Same(driver, node.Driver);
            Assert.Same(device, node.Device);
            Assert.Same(param, node.Parameter);
            Assert.IsType<DatabseNode>(node);

            await driver.CloseAsync(node);
        }
        finally
        {
            if (File.Exists(dbFile)) File.Delete(dbFile);
        }
    }

    [Fact]
    public async Task IoTDatabaseDriver_ReadAsync_WithSqlite()
    {
        var dbFile = Path.GetTempFileName();
        try
        {
            var connStr = $"Data Source={dbFile}";
            var connName = connStr.GetBytes().Crc().GetBytes().ToHex();
            if (!DAL.ConnStrs.ContainsKey(connName))
                DAL.AddConnStr(connName, connStr, null, "SQLite");
            var dal = DAL.Create(connName);

            dal.Execute("CREATE TABLE IF NOT EXISTS IoT_Sensor (Id INTEGER PRIMARY KEY, Name TEXT, Value TEXT)");
            dal.Execute("INSERT INTO IoT_Sensor (Name, Value) VALUES ('Temp', '25.5')");
            dal.Execute("INSERT INTO IoT_Sensor (Name, Value) VALUES ('Humidity', '60')");

            var driver = new IoTDatabaseDriver();
            var param = new DatabaseParameter
            {
                ConnectionString = connStr,
                DatabaseType = DatabaseType.SQLite,
                QuerySql = "SELECT * FROM IoT_Sensor",
                CaptureAll = true,
            };
            var device = new MiniDevice();
            var node = await driver.OpenAsync(device, param);
            Assert.NotNull(node);

            var points = new IPoint[]
            {
                // 列名为 Value，值为 25.5
                new PointModel { Name = "Value", Type = "String" },
            };
            var result = await driver.ReadAsync(node, points);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Values);
            Assert.Equal("25.5", result.Values[0]);

            await driver.CloseAsync(node);
        }
        finally
        {
            if (File.Exists(dbFile)) File.Delete(dbFile);
        }
    }

    [Fact]
    public async Task IoTDatabaseDriver_ReadAsync_CaptureAll()
    {
        var dbFile = Path.GetTempFileName();
        try
        {
            var connStr = $"Data Source={dbFile}";
            var connName = connStr.GetBytes().Crc().GetBytes().ToHex();
            if (!DAL.ConnStrs.ContainsKey(connName))
                DAL.AddConnStr(connName, connStr, null, "SQLite");
            var dal = DAL.Create(connName);

            dal.Execute("CREATE TABLE IF NOT EXISTS IoT_Env (Id INTEGER PRIMARY KEY, Name TEXT, Value TEXT)");
            dal.Execute("INSERT INTO IoT_Env (Name, Value) VALUES ('Temp', '25.5')");
            dal.Execute("INSERT INTO IoT_Env (Name, Value) VALUES ('Humidity', '60')");

            var driver = new IoTDatabaseDriver();
            var param = new DatabaseParameter
            {
                ConnectionString = connStr,
                DatabaseType = DatabaseType.SQLite,
                QuerySql = "SELECT * FROM IoT_Env WHERE Name='Temp'",
                CaptureAll = true,
            };
            var device = new MiniDevice();
            var node = await driver.OpenAsync(device, param);
            Assert.NotNull(node);

            // 不指定点位，CaptureAll=true 时所有列都返回
            var result = await driver.ReadAsync(node, []);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess);

            await driver.CloseAsync(node);
        }
        finally
        {
            if (File.Exists(dbFile)) File.Delete(dbFile);
        }
    }

    [Fact]
    public async Task IoTDatabaseDriver_OpenAsync_NullParameter_Throws()
    {
        var driver = new IoTDatabaseDriver();
        var device = new MiniDevice();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            driver.OpenAsync(device, null!));
    }
}
