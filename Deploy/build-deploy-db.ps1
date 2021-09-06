docker build ..\DbUsers\. -t dbsql:dev
docker tag dbsql:dev us-west2-docker.pkg.dev/utility-descent-185119/webapp-repo/dbsql:dev
docker push us-west2-docker.pkg.dev/utility-descent-185119/webapp-repo/dbsql:dev
kubectl delete -f db.yaml 
kubectl apply -f db.yaml
