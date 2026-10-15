# Multi-stage Dockerfile for Vue 3 client
FROM node:22-alpine AS build
WORKDIR /app

COPY package.json ./
RUN npm install

COPY . ./
RUN npm run build

# Production Static Serve
FROM nginx:alpine AS runtime
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
