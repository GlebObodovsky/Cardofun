$absolutePath = Resolve-Path .

docker build -f $absolutePath\Cardofun.Microservices\Repositories\Cardofun.Microservices.Repositories.Location\dockerfile --force-rm -t cardofunlocation .