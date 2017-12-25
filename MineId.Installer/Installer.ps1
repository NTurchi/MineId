# Import des variables globale
. ./Env.ps1;

# Variable de config
$TITLE_TEXT_COLOR = "Yellow"; 
$MENU_COLOR = "Green";
$ERROR_COLOR = "Red";
$SUBTITLE_COLOR = "Cyan";

# Fonctions utilitaires
function SautDeLigne($nbrLigne) {
	for($i=0; $i -lt $nbrLigne; $i++) {
		Write-Host;	
	}
}

# Afficher un titre encadré
function AfficherTitre($msg) {
	Write-Host "---------------------------------------------------------------" -ForegroundColor $TITLE_TEXT_COLOR;	
	Write-Host $msg -ForegroundColor $TITLE_TEXT_COLOR;	
	Write-Host "---------------------------------------------------------------" -ForegroundColor $TITLE_TEXT_COLOR;	
}

function AfficherSousTitre($msg) {
	Write-Host "-----------------------------" -ForegroundColor $SUBTITLE_COLOR;
	Write-Host $msg -ForegroundColor $SUBTITLE_COLOR;
	Write-Host "-----------------------------" -ForegroundColor $SUBTITLE_COLOR;
}

# Fonctions menu
function AfficherMenu {
	SautDeLigne(1);

	Write-Host "################" -ForegroundColor $MENU_COLOR; 
	Write-Host "#     Menu     #" -ForegroundColor $MENU_COLOR;
	Write-Host "################" -ForegroundColor $MENU_COLOR;

	SautDeLigne(1);

	Write-Host "[1] - Build images";
	Write-Host "[2] - (DEV)  - Run containers";
	Write-Host "[3] - (PROD) - Run containers"; 
	Write-Host "[4] - Add Migration";
	Write-Host "[5] - Clean Container, services...";
	Write-Host "[6] - HARD Clean images";
	Write-Host "[7] - Quit";	
	Write-Host;
	$choix = Read-Host "Choix";
	choixMenu($choix);
}

function CleanMineIdImages($imageTag)
{
	docker rmi --force $(docker images -q $imageTag);
}

function CleanAllContainer
{
	docker rm $(docker ps -a -q);
}

function HardCleanAllImage
{
	docker rmi --force $(docker images -a -q);
}

## DEVELOPMENT IMAGE BUILD

function BuildDevMineIdImage
{
	AfficherSousTitre("[DEV] > BUILD IMAGE .NET CORE");
	# Docker build de l'image de l'API
	docker build --tag nturchi/mineid:latest -f ../MineId/Dockerfile.MineIdDev ../MineId;
}
 
## PRODUCTION IMAGE BUILD

function BuildProdMineIdImage
{
	AfficherSousTitre("[PROD] > BUILD IMAGE .NET CORE");
	# Docker build de l'image de l'API
	docker build --tag nturchi/mineid:latest -f ../MineId/Dockerfile.MineIdProd ../MineId;
}

## DATABASE IMAGE BUILD	

function BuildPgDb
{
	AfficherSousTitre("BUILD IMAGE POSTGRE SQL BDD");
	# Docker build de l'image de la bdd
	docker build -t nturchi/mineid_pgdb:latest -f ./Database/Dockerfile ./Database;
}

## Container run
function RunDevContainer 
{
	AfficherSousTitre("MineId Dev Container");
	$path = $PSCommandPath.Replace("MineId.Installer\Installer.ps1", "MineId");
	Write-Host($path);
	docker run --rm -it -p 5000:80 -v "$($path):/app" nturchi/mineid:latest;
}

function choixMenu($nombre){
	switch($nombre){
		"1" {
			SautDeLigne(1);
			AfficherTitre("Build des images docker");
			SautDeLigne(1);
			AfficherSousTitre("DEVELOPPEMENT");
			Write-Host "[1] MineId";
			SautDeLigne(1);
			AfficherSousTitre("PRODUCTION");
			Write-Host "[11] MineId";
			Write-Host "[13] Toutes";
			SautDeLigne(1);
			AfficherSousTitre("BASE DE DONNEES");
			Write-Host "[21] MineId_PgDB";
			SautDeLigne(1);
			AfficherSousTitre("AUTRES");
			Write-Host "[Q] Revenir au menu";
			SautDeLigne(1);
			$choix = Read-Host "Choix";
			Write-Host;
			switch($choix) {
				# DEVELOPMENT
				"1" {
					BuildDevMineIdImage;
				}
				# PRODUCTION
				"11" {
					BuildProdMineIdImage;
				}
				# DATABASE
				"21" {
					BuildPgDb;
				}
				"13" {
					BuildProdMineIdImage;
					BuildPgDb;
				}
				Default {
					AfficherMenu;
				}
			}
		}
		"2" {
			AfficherTitre("DEV - Run container");
			SautDeLigne(1);
			RunDevContainer;
		}
		"3" {
			AfficherTitre("PROD - Run container");
			SautDeLigne(1);
			AfficherSousTitre("Docker Compose");
			# Docker compose
			docker-compose up; 
		}
		"4" {
			AfficherTitre("Migration bdd");
			SautDeLigne(1);
			$nomMigration = Read-Host "Nom de la migration";
			cd ../MineId | dotnet ef migrations add $nomMigration;
			dotnet ef database update;
		}
		"5" {
			AfficherTitre("Clean des container");
			CleanAllContainer;
			CleanMineIdImages("nturchi/mineid:latest");
		}
		"6" {
			AfficherTitre("Suppression de toute les images de l'ordinateur");
			HardCleanAllImage;
		}
		"7" {
			return;
		}
		Default {
			AfficherMenu;
		}
	}
	AfficherMenu;
}

############################
# PROGRAMME PRINCIPAL
############################

AfficherTitre("Installation complète de MineId");
AfficherMenu;


