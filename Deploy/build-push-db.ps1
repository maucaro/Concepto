type ..\DbUsers\Dockerfile | docker build -f - .. -t dbsql:dev
docker tag dbsql:dev us-west2-docker.pkg.dev/utility-descent-185119/webapp-repo/dbsql:dev
docker push us-west2-docker.pkg.dev/utility-descent-185119/webapp-repo/dbsql:dev

