# -*-coding:Latin-1 -* 

import os;



def cmd(command):
    return os.system(command);

def input_cmd():
    command = raw_input("Enter command to execute : ");
    debug = "Commande à executer : " + command;
    print(debug);
    cmd(command);
    
## Liée au menu du script d'installation
def choixMenu(nombre):
    if(nombre == "1"):
        print(" ");
        AfficherTitre("Build des images docker");
        print(" ");
        AfficherSousTitre("DEVELOPPEMENT");
        print("[1] MineId");
        print(" ");
        AfficherSousTitre("PRODUCTION");
        print("[11] MineId");
        print("[13] Toutes");
        print(" ");
        AfficherSousTitre("BASE DE DONNEES");
        print("[21] MineId_PgDB");
        print(" ");
        AfficherSousTitre("AUTRES");
        print("[Q] Revenir au menu");
        print(" ");
        choix = raw_input("Choix : ");
        print(" ");
        if (choix == "1"):
        # DEVELOPMENT
            BuildDevMineIdImage();
        # PRODUCTION
        elif (choix == "11"):
            BuildProdMineIdImage();
        # DATABASE
        elif (choix == "21"):
            BuildPgDb();
        elif (choix == "13"):
            BuildProdMineIdImage();
            BuildPgDb();
        else:
            AfficherMenu();

    elif(nombre == "2"):
        AfficherTitre("DEV - Run container");
        print(" ");
        RunDevContainer();

    elif(nombre == "3"):
        AfficherTitre("PROD - Run container");
        print(" ");
        AfficherSousTitre("Docker Compose");
        # Docker compose
        cmd("docker-compose up");

    elif(nombre == "4"):
        AfficherTitre("Migration bdd");
        print(" ");
        nomMigration = raw_input("Nom de la migration");
        cmd("cd ../MineId | dotnet ef migrations add" + nomMigration);
        cmd("dotnet ef database update");

    elif (nombre == "5"):
        AfficherTitre("Clean des container");
        CleanAllContainer();
        CleanMineIdImages("nturchi/mineid:latest");

    elif (nombre == "6"):
        AfficherTitre("Suppression de toute les images de l'ordinateur");
        HardCleanAllImage();

    elif (nombre == "7"):
        return;

    else:
        AfficherMenu();
    
	AfficherMenu();

def afficherMenu():
    go = True;
    while(go):
        print("################");
        print("#     Menu     #");
        print("################\n");

        print("[1] - Build images");
        print("[2] - (DEV)  - Run containers");
        print("[3] - (PROD) - Run containers"); 
        print("[4] - Add Migration");
        print("[5] - Clean Container, services...");
        print("[6] - HARD Clean images");
        print("[7] - Quit");	
        print(" ");
        choix = raw_input("Choix : ");
        choixMenu(choix);

## Command Docker build / run 
def CleanMineIdImages(imageTag):
    cmd("docker rmi --force `docker images -q " + imageTag + "`");

def CleanAllContainer():
    cmd("docker rm --force `docker ps -a -q`");


def HardCleanAllImage():
    cmd("docker rmi --force `docker images -a -q`");

def BuildDevMineIdImage():
    AfficherSousTitre("[DEV] > BUILD IMAGE .NET CORE");
    # Docker build de l'image de l'API
    cmd("docker build --tag nturchi/mineid:latest -f ../MineId/Dockerfile.MineIdDev ../MineId");

 
## PRODUCTION IMAGE BUILD

def BuildProdMineIdImage():
	AfficherSousTitre("[PROD] > BUILD IMAGE .NET CORE");
	# Docker build de l'image de l'API
	cmd("docker build --tag nturchi/mineid:latest -f ../MineId/Dockerfile.MineIdProd ../MineId");

## DATABASE IMAGE BUILD	

def BuildPgDb():
	AfficherSousTitre("BUILD IMAGE POSTGRE SQL BDD");
	# Docker build de l'image de la bdd
	cmd("docker build -t nturchi/mineid_pgdb:latest -f ./Database/Dockerfile ./Database");

# Container run
def RunDevContainer():
    AfficherSousTitre("MineId Dev Container");
    path = os.getcwd().replace("MineId.Installer", "MineId");
    print("DEBUG" + path);
    cmd("docker run --rm -it -p 5000:80 -v " + path + ":/app nturchi/mineid:latest");



# Command propre à l'affichage 
def AfficherTitre(msg):
	print("---------------------------------------------------------------");	
	print(msg);	
	print("---------------------------------------------------------------");	


def AfficherSousTitre(msg):
	print("-----------------------------");
	print(msg);
	print("-----------------------------");


# Entrée principale de l'application
def main():
    afficherMenu();

main();