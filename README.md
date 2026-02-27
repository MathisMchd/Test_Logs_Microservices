


# Logs entre Microservices, ElasticSearch et Kibana

Le but est de centraliser les logs des microservices dans ElasticSearch et de les visualiser avec Kibana

## Prérequis 

 - Docker

## Packages
 - Serilog (Microservices) (https://serilog.net/)
 - OpenTelemetry (https://opentelemetry.io/)

## Lancement du projet

A la racine du projet : 

Pour lancer elastic et kibana : 

```
docker-compose -f docker-compose.elasticsearch.yml up -d
```




Pour lancer les microservices :

```
docker-compose -f docker-compose.microservices.yml up --build
```


## Postman 

En POST : 
 - http://localhost:5001/job/start-job
 - http://localhost:5001/job/double-job

## Visualisation des logs avec Kibana

 - http://localhost:5601/


Créer une vue pour visualiser les logs dans les menus à gauche : `Analytics > Dicover`.
Remarque : Il vous faut au moins avoir utilisé les microservices une fois (pour avoir des index et pouvoir créer la vue)

- Nom : `Microservices`
- Index pattern pour cibler les sources : `logs-*`
- Timestamp field : `@timestamp`

![Vue analytics logs microservices png](./Vue_analytics_logs_microservices.png)


Sauvegarder la vue.

_Remarques_ : si vous avez déjà lancé et appelé les microservices, vous allez voir deux sources :
 - logs-microservice-a-default
 - logs-microservice-b-default


## Embeded Dashboard

https://www.elastic.co/blog/how-to-embed-kibana-dashboards


## Notes

CorrelationID non nécessaire, déjà fait par le traceId
