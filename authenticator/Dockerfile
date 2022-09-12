FROM node:18-slim AS builder
WORKDIR /build
RUN npm install -g typescript
COPY package.json package-lock.json tsconfig.json src/*.ts ./
RUN npm install && npx tsc

FROM node:18-slim
WORKDIR /app
COPY --from=builder /build/dist/* ./
COPY --from=builder /build/node_modules ./node_modules
EXPOSE 8080
CMD [ "node", "app.js" ]

