echo Starting Install Script
cd new
git clone git@github.com:mjwsteenbergen/ApiLibs.git ApiLibs
cd ApiLibs
nuget restore
cd ..
nuget restore
xbuild TelegramBot.sln
