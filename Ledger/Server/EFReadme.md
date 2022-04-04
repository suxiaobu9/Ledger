# EFReadMe

```powershell
# 有 appsettings.json 的專案
cd Ledger\Server

# -o 指定輸出目錄
# --force 覆蓋舊的檔案
# --namespace 指定產生檔案的 namespace
# --no-onconfiguring 不要產生 DbContext.OnConfiguring

# 手動給連線資訊
# dotnet ef dbcontext scaffold "Server=;Database=;user id=;password=" Microsoft.EntityFrameworkCore.SqlServer -o ..\Shared\EF\ --force --namespace "ITRI.CTMS.Model.EF" --no-onconfiguring

# 吃 appsettings.json 裡面的值，也吃的到 user secrets
dotnet ef dbcontext scaffold "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -o ..\Shared\EF\ --force --no-onconfiguring


```
