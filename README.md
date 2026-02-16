


# Microservices, ElasticSearch et Kibana


A la racine du projet : 

Pour lancer elastic et kibana : 

```
docker-compose -f docker-compose.elasticsearch.yml up -d
```




Pour lancer les microservices :

```
docker-compose -f docker-compose.microservices.yml up --build
```

curl http://localhost:9200/_cluster/health


## Notes

CorrelationID non nécessaire, déjà fait par le traceId