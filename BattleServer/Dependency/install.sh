sudo yum install yum-utils 
sudo rpm --import "http://keyserver.ubuntu.com/pks/lookup?op=get&search=0x3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF"
sudo yum-config-manager --add-repo http://download.mono-project.com/repo/centos/
sudo yum install -y mono-complete
sudo chmod 755 BattleServerRun.sh
sudo chmod 755 BattleServer.exe

# download and install agent
wget https://s3.amazonaws.com/amazoncloudwatch-agent/windows/amd64/latest/amazon-cloudwatch-agent.msi
msiexec /i amazon-cloudwatch-agent.msi
