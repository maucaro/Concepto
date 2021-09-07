docker build -f "C:\Users\maucaro\source\repos\Concepto\WebApp\Dockerfile" --force-rm -t webapp:dev "C:\Users\maucaro\source\repos\Concepto"
docker tag webapp:dev us-west2-docker.pkg.dev/utility-descent-185119/webapp-repo/webapp:dev
docker push us-west2-docker.pkg.dev/utility-descent-185119/webapp-repo/webapp:dev

