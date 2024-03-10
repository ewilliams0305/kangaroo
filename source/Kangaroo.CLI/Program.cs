using Cocona;
using Kangaroo.CLI.Commands;

var app = CoconaLiteApp.Create(args);

app.AddCommands<RangeScanCommand>();
app.AddCommands<AdapterScanCommand>();
app.AddCommands<SubnetScanCommand>();

app.Run();