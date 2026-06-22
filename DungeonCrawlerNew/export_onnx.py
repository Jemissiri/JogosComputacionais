#!/usr/bin/env python3
"""Convert boss_run_04 .pt checkpoint to .onnx for Unity inference.

Bypasses the dynamo-based exporter (PyTorch 2.x) by forcing the legacy
torch.onnx.export path, which is what ML-Agents 1.1.0 was designed for.
"""

import torch
from types import SimpleNamespace
from mlagents.trainers.torch_entities.networks import SimpleActor
from mlagents.trainers.torch_entities.model_serialization import (
    ModelSerializer, exporting_to_onnx
)
from mlagents.trainers.settings import NetworkSettings, SerializationSettings
from mlagents_envs.base_env import (
    ActionSpec, ObservationSpec, ObservationType, DimensionProperty, BehaviorSpec
)

PT_PATH   = "results/boss_run_04/BossAgent/BossAgent-499896.pt"
ONNX_PATH = "results/boss_run_04/BossAgent/BossAgent.onnx"

obs_specs = [ObservationSpec(
    shape=(17,),
    observation_type=ObservationType.DEFAULT,
    name="VectorSensor",
    dimension_property=(DimensionProperty.NONE,)
)]

action_spec    = ActionSpec.create_hybrid(continuous_size=2, discrete_branches=(3,))
behavior_spec  = BehaviorSpec(observation_specs=obs_specs, action_spec=action_spec)
network_settings = NetworkSettings(normalize=True, hidden_units=256, num_layers=3)

actor = SimpleActor(obs_specs, network_settings, action_spec)

raw = torch.load(PT_PATH, map_location="cpu")["Policy"]
model_state = {k: v for k, v in raw.items() if isinstance(v, torch.Tensor)}
actor.load_state_dict(model_state)
actor.eval()

policy = SimpleNamespace(behavior_spec=behavior_spec, export_memory_size=0, actor=actor)
ms = ModelSerializer(policy)

# Force the legacy (non-dynamo) ONNX exporter
with exporting_to_onnx():
    torch.onnx.export(
        actor,
        ms.dummy_input,
        ONNX_PATH,
        opset_version=SerializationSettings.onnx_opset,
        input_names=ms.input_names,
        output_names=ms.output_names,
        dynamic_axes=ms.dynamic_axes,
        dynamo=False,   # force legacy exporter — required for ML-Agents 1.1.0
    )

print(f"Exported: {ONNX_PATH}")
