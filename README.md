


# Logs entre Microservices, ElasticSearch et Kibana

Le but est de centraliser les logs des microservices dans ElasticSearch et de les visualiser avec Kibana

## Pr�requis 

 - Docker

## Packages
 - Serilog (Microservices) (https://serilog.net/)
 - OpenTelemetry (https://opentelemetry.io/)

 - Elastic
 - Kibana

 - Elastic.Serilog.Sinks pour elasticSearch et serilog
 - 
## Lancement du projet

A la racine du projet : 

Creer le réseau si pas déjà existant
```
docker network create microservices-network
```

Pour lancer elastic et kibana (sous `/Microservices`): 


```
docker-compose -f docker-compose.elasticsearch.yml up --build -d
```




Pour lancer les microservices (sous `/Microservices`):

```
docker-compose -f docker-compose.microservices.yml up --build
```


## Postman 

En POST : 
 - http://localhost:5001/job/start-job
 - http://localhost:5001/job/double-job

## Visualisation des logs avec Kibana

 - http://localhost:5601/


Cr�er une vue pour visualiser les logs dans les menus � gauche : `Analytics > Dicover`.
Remarque : Il vous faut au moins avoir utilis� les microservices une fois (pour avoir des index et pouvoir cr�er la vue)

- Nom : `Microservices`
- Index pattern pour cibler les sources : `logs-*`
- Timestamp field : `@timestamp`

![Vue analytics logs microservices png](./Vue_analytics_logs_microservices.png)


Sauvegarder la vue.

_Remarques_ : si vous avez d�j� lanc� et appel� les microservices, vous allez voir deux sources :
 - logs-microservice-a-default
 - logs-microservice-b-default


## Embeded Dashboard

https://www.elastic.co/blog/how-to-embed-kibana-dashboards


## Notes

CorrelationID non nécessaire, déjà fait par le traceId
