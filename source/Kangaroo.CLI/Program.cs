using Cocona;
using Dumpify;
using Kangaroo.CLI.Commands;

DumpConfig.Default.TypeNamingConfig.ShowTypeNames = false;
DumpConfig.Default.ColorConfig.PropertyNameColor = DumpColor.FromHexString("#4388f7");

var app = CoconaLiteApp.Create(args);

app.AddCommands<RangeScanCommand>();
app.AddCommands<AdapterScanCommand>();
app.AddCommands<SubnetScanCommand>();

app.Run();