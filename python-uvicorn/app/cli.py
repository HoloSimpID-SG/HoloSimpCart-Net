import os
import uvicorn


def serve():
  port_env = os.getenv("UVICORN_PORT", "8000").strip()
  port = int(port_env) if port_env else 8000
  # cmd = ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", port]
  uvicorn.run("app.main:app", host="0.0.0.0", port=port)
