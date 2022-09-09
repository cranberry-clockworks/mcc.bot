FROM node:18-slim

WORKDIR /usr/app

COPY package*.json ./
RUN npm install

COPY dist/* .

EXPOSE 3000

CMD [ "node", "app.js" ]

