#!/bin/bash

echo '127.0.0.1 kafka' | sudo tee -a /etc/hosts
echo '127.0.0.1 zookeeper' | sudo tee -a /etc/hosts

BASHRC="$HOME/.bashrc"
MARKER="# Kafka CLI commands"

KAFKA_COMMANDS=$(cat <<EOF
$MARKER
kafka-topics() {
  docker exec -it kafka kafka-topics "\$@"
}

kafka-console-producer() {
  docker exec -it kafka kafka-console-producer "\$@"
}

kafka-console-consumer() {
  docker exec -it kafka kafka-console-consumer "\$@"
}
EOF
)

# Add commands to .bashrc if not already present
if ! grep -q "$MARKER" "$BASHRC"; then
  echo "$KAFKA_COMMANDS" >> "$BASHRC"
  echo "Kafka commands added to .bashrc."
else
  echo "Kafka commands already present in .bashrc."
fi

# Reload .bashrc
source "$BASHRC"
