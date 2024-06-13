FROM node:22 AS build
WORKDIR /build
COPY . .
RUN npm install
ENV ANALYTICS_CLARITY_ID=${ANALYTICS_CLARITY_ID}
RUN npm run build

FROM node:22 AS final
WORKDIR /app
COPY --from=build /build/dist/client .
EXPOSE 80
CMD [ "npx", "serve" , "-p", "80", "-s", "."]