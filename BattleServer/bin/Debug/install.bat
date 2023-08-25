msiexec /i ./amazon-cloudwatch-agent.msi
& "C:\Program Files\Amazon\AmazonCloudWatchAgent\amazon-cloudwatch-agent-ctl.ps1" -a fetch-config -m ec2 -c amazon-cloudwatch-agent.json -s