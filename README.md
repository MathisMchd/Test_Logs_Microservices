


# Microservices, ElasticSearch et Kibana


docker-compose -f docker-compose.elasticsearch.yml up -d


curl http://localhost:9200/_cluster/health


docker-compose -f docker-compose.microservices.yml up --build
